﻿using System;
using System.Globalization;
using System.Linq.Expressions;

namespace LinqToDB.DataProvider.Sybase.Translation
{
	using Common;
	using SqlQuery;
	using Linq.Translation;

	public class SybaseMemberTranslator : ProviderMemberTranslatorDefault
	{
		class SqlTypesTranslation : SqlTypesTranslationDefault
		{
			protected override Expression? ConvertDateTime2(ITranslationContext translationContext, MemberExpression memberExpression, TranslationFlags translationFlags)
				=> MakeSqlTypeExpression(translationContext, memberExpression, t => t.WithDataType(DataType.DateTime));

			protected override Expression? ConvertDateTimeOffset(ITranslationContext translationContext, MemberExpression memberExpression, TranslationFlags translationFlags)
				=> MakeSqlTypeExpression(translationContext, memberExpression, t => t.WithDataType(DataType.DateTime));
		}

		public class DateFunctionsTranslator : DateFunctionsTranslatorBase
		{
			public static string? DatePartToStr(Sql.DateParts part)
			{
				return part switch
				{
					Sql.DateParts.Year => "year",
					Sql.DateParts.Quarter => "quarter",
					Sql.DateParts.Month => "month",
					Sql.DateParts.DayOfYear => "dayofyear",
					Sql.DateParts.Day => "day",
					Sql.DateParts.Week => "week",
					Sql.DateParts.WeekDay => "weekday",
					Sql.DateParts.Hour => "hour",
					Sql.DateParts.Minute => "minute",
					Sql.DateParts.Second => "second",
					Sql.DateParts.Millisecond => "millisecond",
					_ => null
				};
			}

			protected override ISqlExpression? TranslateDateTimeDatePart(ITranslationContext translationContext, TranslationFlags translationFlag, ISqlExpression dateTimeExpression, Sql.DateParts datepart)
			{
				var partStr = DatePartToStr(datepart);

				if (partStr == null)
					return null;

				var factory   = translationContext.ExpressionFactory;
				var intDbType = factory.GetDbDataType(typeof(int));

				var resultExpression = factory.Function(intDbType, "DatePart", factory.Fragment(intDbType, partStr), dateTimeExpression);

				return resultExpression;
			}

			protected override ISqlExpression? TranslateDateTimeDateAdd(ITranslationContext translationContext, TranslationFlags translationFlag, ISqlExpression dateTimeExpression, ISqlExpression increment,
				Sql.DateParts                                                       datepart)
			{
				var factory  = translationContext.ExpressionFactory;
				var dateType = factory.GetDbDataType(dateTimeExpression);

				var partStr = DatePartToStr(datepart);

				if (partStr == null)
				{
					return null;
				}

				var resultExpression = factory.Function(dateType, "DateAdd", factory.Fragment(factory.GetDbDataType(typeof(string)), partStr), increment, dateTimeExpression);
				return resultExpression;
			}

			protected override ISqlExpression? TranslateMakeDateTime(
				ITranslationContext translationContext,
				DbDataType          resulType,
				ISqlExpression      year,
				ISqlExpression      month,
				ISqlExpression      day,
				ISqlExpression?     hour,
				ISqlExpression?     minute,
				ISqlExpression?     second,
				ISqlExpression?     millisecond)
			{
				var factory        = translationContext.ExpressionFactory;
				var stringDataType = factory.GetDbDataType(typeof(string)).WithDataType(DataType.VarChar);
				var intDataType    = factory.GetDbDataType(typeof(int));

				ISqlExpression CastToLength(ISqlExpression expression, int stringLength)
				{
					return factory.Cast(expression, stringDataType.WithLength(stringLength));
				}

				ISqlExpression PartExpression(ISqlExpression expression, int padSize)
				{
					if (translationContext.TryEvaluate(expression, out var expressionValue) && expressionValue is int intValue)
					{
						var padLeft = intValue.ToString(CultureInfo.InvariantCulture).PadLeft(padSize, '0');
						return factory.Value(stringDataType.WithLength(padLeft.Length), padLeft);
					}

					return factory.Function(stringDataType, "RIGHT",
						factory.Concat(factory.Value(stringDataType, "0"), CastToLength(expression, padSize)),
						factory.Value(intDataType, padSize));
				}

				var yearString  = PartExpression(year, 4);
				var monthString = PartExpression(month, 2);
				var dayString   = PartExpression(day, 2);

				var resultExpression = factory.Concat(
					yearString, factory.Value(stringDataType, "-"),
					monthString, factory.Value(stringDataType, "-"), dayString);

				if (hour != null || minute != null || second != null || millisecond != null)
				{
					hour        ??= factory.Value(intDataType, 0);
					minute      ??= factory.Value(intDataType, 0);
					second      ??= factory.Value(intDataType, 0);
					millisecond ??= factory.Value(intDataType, 0);

					resultExpression = factory.Concat(
						resultExpression,
						factory.Value(stringDataType, " "),
						PartExpression(hour, 2), factory.Value(stringDataType, ":"),
						PartExpression(minute, 2), factory.Value(stringDataType, ":"),
						PartExpression(second, 2), factory.Value(stringDataType, "."),
						PartExpression(millisecond, 3)
					);
				}

				resultExpression = factory.Cast(resultExpression, resulType);

				return resultExpression;
			}

			protected override ISqlExpression? TranslateDateTimeTruncationToDate(ITranslationContext translationContext, ISqlExpression dateExpression, TranslationFlags translationFlags)
			{
				// CONVERT(date, your_datetime_column)

				var convertFunc = new SqlFunction(dateExpression.SystemType!, "CONVERT", new SqlDataType(DataType.Date), dateExpression);
				return convertFunc;
			}

			protected override ISqlExpression? TranslateSqlGetDate(ITranslationContext translationContext, TranslationFlags translationFlags)
			{
				var factory = translationContext.ExpressionFactory;
				return factory.Function(factory.GetDbDataType(typeof(DateTime)), "GetDate");
			}
		}

		protected override IMemberTranslator CreateSqlTypesTranslator()
		{
			return new SqlTypesTranslation();
		}

		protected override IMemberTranslator CreateDateMemberTranslator()
		{
			return new DateFunctionsTranslator();
		}

		protected override ISqlExpression? TranslateNewGuidMethod(ITranslationContext translationContext, TranslationFlags translationFlags)
		{
			var factory  = translationContext.ExpressionFactory;
			var timePart = factory.NonPureFunction(factory.GetDbDataType(typeof(Guid)), "NewID", factory.Value(1));

			return timePart;
		}
	}
}
