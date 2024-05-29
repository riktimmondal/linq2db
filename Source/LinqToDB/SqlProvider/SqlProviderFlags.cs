﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

namespace LinqToDB.SqlProvider
{
	using Common;
	using DataProvider;
	using SqlQuery;

	[DataContract]
	public sealed class SqlProviderFlags
	{
		/// <summary>
		/// Flags for use by external providers.
		/// </summary>
		[DataMember(Order = 1)]
		public List<string> CustomFlags { get; set; } = new List<string>();

		/// <summary>
		/// Indicates that provider (not database!) uses positional parameters instead of named parameters (parameter values assigned in order they appear in query, not by parameter name).
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order =  2)]
		public bool        IsParameterOrderDependent      { get; set; }

		/// <summary>
		/// Indicates that TAKE/TOP/LIMIT could accept parameter.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order =  3)]
		public bool        AcceptsTakeAsParameter         { get; set; }
		/// <summary>
		/// Indicates that TAKE/LIMIT could accept parameter only if also SKIP/OFFSET specified.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order =  4)]
		public bool        AcceptsTakeAsParameterIfSkip   { get; set; }
		/// <summary>
		/// Indicates support for TOP/TAKE/LIMIT paging clause.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order =  5)]
		public bool        IsTakeSupported                { get; set; }
		/// <summary>
		/// Indicates support for SKIP/OFFSET paging clause (parameter) without TAKE clause.
		/// Provider could set this flag even if database not support it if emulates missing functionality.
		/// E.g. : <c>TAKE [MAX_ALLOWED_VALUE] SKIP skip_value </c>
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order =  6)]
		public bool        IsSkipSupported                { get; set; }
		/// <summary>
		/// Indicates support for SKIP/OFFSET paging clause (parameter) only if also TAKE/LIMIT specified.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order =  7)]
		public bool        IsSkipSupportedIfTake          { get; set; }
		/// <summary>
		/// Indicates supported TAKE/LIMIT hints.
		/// Default (set by <see cref="DataProviderBase"/>): <c>null</c> (none).
		/// </summary>
		[DataMember(Order =  8)]
		public TakeHints?  TakeHintsSupported              { get; set; }
		/// <summary>
		/// Indicates support for paging clause in subquery.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order =  9)]
		public bool        IsSubQueryTakeSupported        { get; set; }

		/// <summary>
		/// Indicates that provider has issue with any JOIN to subquery which has TOP statement.
		/// Default <c>false</c>.
		/// </summary>
		/// <remarks>Currently use as workaround over Sybase bug.</remarks>
		[DataMember(Order = 10)]
		public bool IsJoinDerivedTableWithTakeInvalid { get; set; }

		/// <summary>
		/// Indicates support for paging clause in correlated subquery.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 11)]
		public bool IsCorrelatedSubQueryTakeSupported { get; set; }

		/// <summary>
		/// Indicates that provider supports JOIN without condition ON 1=1.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 12)]
		public bool IsSupportsJoinWithoutCondition { get; set; }
		
		/// <summary>
		/// Indicates support for skip clause in column expression subquery.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order =  13)]
		public bool        IsSubQuerySkipSupported        { get; set; }

		/// <summary>
		/// Indicates support for scalar subquery in select list.
		/// E.g. <c>SELECT (SELECT TOP 1 value FROM some_table) AS MyColumn, ...</c>
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 14)]
		public bool        IsSubQueryColumnSupported      { get; set; }
		/// <summary>
		/// Indicates support of <c>ORDER BY</c> clause in sub-queries.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 15)]
		public bool        IsSubQueryOrderBySupported     { get; set; }
		/// <summary>
		/// Indicates that database supports count subquery as scalar in column.
		/// <code>SELECT (SELECT COUNT(*) FROM some_table) FROM ...</code>
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 16)]
		public bool        IsCountSubQuerySupported       { get; set; }

		/// <summary>
		/// Indicates that provider requires explicit output parameter for insert with identity queries to get identity from database.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 17)]
		public bool        IsIdentityParameterRequired    { get; set; }
		/// <summary>
		/// Indicates support for OUTER/CROSS APPLY.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 18)]
		public bool        IsApplyJoinSupported           { get; set; }
		/// <summary>
		/// Indicates support for CROSS APPLY supports condition LATERAL JOIN for example.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 19)]
		public bool IsCrossApplyJoinSupportsCondition { get; set; }
		/// <summary>
		/// Indicates support for OUTER APPLY supports condition LATERAL JOIN for example.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 20)]
		public bool IsOuterApplyJoinSupportsCondition { get; set; }
		/// <summary>
		/// Indicates support for single-query insert-or-update operation support.
		/// Otherwise two separate queries used to emulate operation (update, then insert if nothing found to update).
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 21)]
		public bool        IsInsertOrUpdateSupported      { get; set; }
		/// <summary>
		/// Indicates that provider could share parameter between statements in multi-statement batch.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 22)]
		public bool        CanCombineParameters           { get; set; }
		/// <summary>
		/// Specifies limit of number of values in single <c>IN</c> predicate without splitting it into several IN's.
		/// Default (set by <see cref="DataProviderBase"/>): <c>int.MaxValue</c> (basically means there is no limit).
		/// </summary>
		[DataMember(Order = 23)]
		public int         MaxInListValuesCount           { get; set; }

		/// <summary>
		/// If <c>true</c>, removed record fields in OUTPUT clause of DELETE statement should be referenced using
		/// table with special name (e.g. DELETED or OLD). Otherwise fields should be referenced using target table.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 24)]
		public bool        OutputDeleteUseSpecialTable    { get; set; }
		/// <summary>
		/// If <c>true</c>, added record fields in OUTPUT clause of INSERT statement should be referenced using
		/// table with special name (e.g. INSERTED or NEW). Otherwise fields should be referenced using target table.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 25)]
		public bool        OutputInsertUseSpecialTable    { get; set; }
		/// <summary>
		/// If <c>true</c>, OUTPUT clause supports both OLD and NEW data in UPDATE statement using tables with special names.
		/// Otherwise only current record fields (after update) available using target table.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 26)]
		public bool        OutputUpdateUseSpecialTables   { get; set; }

		/// <summary>
		/// Indicates support for CROSS JOIN.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 27)]
		public bool        IsCrossJoinSupported              { get; set; }

		/// <summary>
		/// Indicates support of CTE expressions.
		/// If provider does not support CTE, unsuported exception will be thrown when using CTE.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 28)]
		public bool IsCommonTableExpressionsSupported     { get; set; }

		/// <summary>
		/// Indicates support for aggregate functions in ORDER BY statement.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 29)]
		public bool IsOrderByAggregateFunctionsSupported  { get; set; }

		/// <summary>
		/// Provider supports EXCEPT ALL, INTERSECT ALL set operators. Otherwise they will be emulated.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 30)]
		public bool IsAllSetOperationsSupported           { get; set; }

		/// <summary>
		/// Provider supports EXCEPT, INTERSECT set operators. Otherwise it will be emulated.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 31)]
		public bool IsDistinctSetOperationsSupported      { get; set; }

		/// <summary>
		/// Provider supports aggregated expression with Outer reference
		/// <code>
		/// SELECT
		/// (
		///		SELECT SUM(inner.FieldX + outer.FieldOuter)
		///		FROM table2 inner
		/// ) AS Sum_Column
		/// FROM table1 outer
		///</code>
		/// Otherwise aggeragated expression will be wrapped in subquery and aggregate function will be applied to subquery column.
		/// <code>
		/// SELECT
		/// (
		///		SELECT
		///			SUM(sub.Column)
		///		FROM
		///			(
		///				SELECT inner.FieldX + outer.FieldOuter AS Column
		///				FROM table2 inner
		///			) sub
		/// ) AS Sum_Column
		/// FROM table1 outer
		///</code>
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 32)]
		public bool AcceptsOuterExpressionInAggregate { get; set; }

		/// <summary>
		/// Indicates support for following UPDATE syntax:
		/// <code>
		/// UPDATE A
		/// SET ...
		/// FROM B
		/// </code>
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 33)]
		public bool IsUpdateFromSupported             { get; set; }

		/// <summary>
		/// Provider supports Naming Query Blocks
		/// <code>
		/// QB_NAME(qb)
		/// </code>
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 34)]
		public bool IsNamingQueryBlockSupported       { get; set; }

		/// <summary>
		/// Indicates that provider supports window functions.
		/// Default value: <c>true</c>.
		/// </summary>
		[DataMember(Order = 35)]
		public bool IsWindowFunctionsSupported { get; set; }

		/// <summary>
		/// Used when there is query which needs several additional database requests for completing query (e.g. eager load or client-side GroupBy).
		/// Default (set by <see cref="DataProviderBase"/>): <see cref="IsolationLevel.RepeatableRead"/>.
		/// </summary>
		[DataMember(Order = 36)]
		public IsolationLevel DefaultMultiQueryIsolationLevel { get; set; }

		/// <summary>
		/// Provider support Row Constructor `(1, 2, 3)` in various positions (flags)
		/// Default (set by <see cref="DataProviderBase"/>): <see cref="RowFeature.None"/>.
		/// </summary>
		[DataMember(Order = 37), DefaultValue(RowFeature.None)]
		public RowFeature RowConstructorSupport { get; set; }

		/// <summary>
		/// Default value: <c>false</c>.
		/// </summary>
		[DataMember(Order = 38)]
		public bool DoesNotSupportCorrelatedSubquery { get; set; }

		/// <summary>
		/// Default value: <c>false</c>.
		/// </summary>
		[DataMember(Order = 39)]
		public bool IsExistsPreferableForContains   { get; set; }

		/// <summary>
		/// Provider supports ROW_NUMBER OVER () without ORDER BY
		/// Default value: <c>true</c>.
		/// </summary>
		[DataMember(Order = 40), DefaultValue(true)]
		public bool IsRowNumberWithoutOrderBySupported { get; set; } = true;

		/// <summary>
		/// Provider supports condition in subquery which references parent table
		/// Default value: <c>true</c>.
		/// </summary>
		[DataMember(Order = 41), DefaultValue(true)]
		public bool IsSubqueryWithParentReferenceInJoinConditionSupported { get; set; } = true;

		/// <summary>
		/// Provider supports column subqueries which references outer scope when nesting is more than 1
		/// Default value: <c>true</c>.
		/// </summary>
		[DataMember(Order = 42), DefaultValue(true)]
		public bool IsColumnSubqueryWithParentReferenceSupported { get; set; } = true;

		/// <summary>
		/// Provider supports column subqueries which references outer scope when nesting is more than 1
		/// </summary>
		/// <remarks>
		/// Used only for Oracle 11. liq2db emulates Take(n) via 'ROWNUM' and it cause additional nesting.
		/// Default value: <c>true</c>.
		/// </remarks>
		[DataMember(Order = 43), DefaultValue(true)]
		public bool IsColumnSubqueryWithParentReferenceAndTakeSupported { get; set; } = true;

		/// <summary>
		/// Workaround over Oracle's bug with subquery in column list which contains parent table column with IS NOT NULL condition.
		/// Default value: <c>false</c>.
		/// </summary>
		/// <remarks>
		/// See Issue3557Case1 test.
		/// </remarks>
		[DataMember(Order = 44), DefaultValue(false)]
		public bool IsColumnSubqueryShouldNotContainParentIsNotNull { get; set; } = false;

		/// <summary>
		/// Provider supports INNER JOIN with condition inside Recursive CTE, currently not supported only by DB2
		/// Default value: <c>true</c>.
		/// </summary>
		[DataMember(Order = 45), DefaultValue(true)]
		public bool IsRecursiveCTEJoinWithConditionSupported { get; set; } = true;

		/// <summary>
		/// Provider supports INNER JOIN inside OUTER JOIN. For example:
		/// <code>
		/// LEFT JOIN table1 ON ...
		///	   INNER JOIN query ON ...
		/// </code>
		///
		/// Otherwise the following query will be left:
		/// <code>
		/// LEFT JOIN (
		///	   SELECT ...
		///	   FROM table1
		///	   INNER JOIN query ON ...
		/// )
		/// </code>
		/// Default: <c>true</c>.
		/// <remarks>
		/// Currently not supported only by Access.
		/// </remarks>
		/// </summary>
		[DataMember(Order = 46), DefaultValue(true)]
		public bool IsOuterJoinSupportsInnerJoin { get; set; } = true;

		/// <summary>
		/// Indicates that provider supports JOINS in FROM clause which have several tables
		/// <code>
		/// SELECT ...
		/// FROM table1
		///	   INNER JOIN query ON ...
		///    , table2
		/// </code>
		/// Otherwise the following query will be generated:
		/// <code>
		/// SELECT ...
		/// FROM (
		///		SELECT ...
		///		FROM table1, table2
		/// ) S
		///	   INNER JOIN query ON ... ,
		/// FROM table2
		/// </code>
		/// Default: <c>true</c>.
		/// <remarks>
		/// Currently not supported only by Access.
		/// </remarks>
		/// </summary>
		[DataMember(Order = 47), DefaultValue(true)]
		public bool IsMultiTablesSupportsJoins { get; set; } = true;

		/// <summary>
		/// Indicates that top level CTE query supports ORDER BY.
		/// Default value: <c>true</c>.
		/// </summary>
		[DataMember(Order = 48), DefaultValue(true)]
		public bool IsCTESupportsOrdering { get; set; } = true;

		/// <summary>
		/// Indicates that provider has bug in LEFT join translator.
		/// In the following example <b>can_be_null</b> is always not null, which is wrong.
		/// <code>
		/// SELECT
		///		can_be_null
		/// FROM Some
		/// LEFT JOIN 1 as can_be_null, * FROM Other
		///		ON ...
		/// </code>
		/// As workaround translator is trying to check for nullability all projected fields.
		/// Default value: <c>false</c>.
		/// </summary>
		[DataMember(Order =  49)]
		public bool IsAccessBuggyLeftJoinConstantNullability { get; set; }

		/// <summary>
		/// Indicates that provider supports boolean type.
		/// Default value: <c>false</c>.
		/// </summary>
		[DataMember(Order =  50)]
		public bool SupportsBooleanComparison { get; set; }

		/// <summary>
		/// Provider supports nested joins
		/// <code>
		/// A JOIN (B JOIN C ON ?) ON ?
		/// </code>
		/// otherwise nested join replaced with sub-query
		/// <code>
		/// A JOIN (SELECT ? FROM B JOIN C ON ?) ON ?
		/// </code>.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 51), DefaultValue(true)]
		public bool IsNestedJoinsSupported { get; set; } = true;

		/// <summary>
		/// Provider supports COUNT(DISTINCT column) function. Otherwise, it will be emulated.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 52)]
		public bool IsCountDistinctSupported { get; set; }

		/// <summary>
		/// Provider supports SUM/AVG/MIN/MAX(DISTINCT column) function. Otherwise, it will be emulated.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 53)]
		public bool IsAggregationDistinctSupported { get; set; }

		/// <summary>
		/// Provider supports SUM/AVG/MIN/MAX(DISTINCT column) function. Otherwise, it will be emulated.
		/// Default (set by <see cref="DataProviderBase"/>): <c>true</c>.
		/// </summary>
		[DataMember(Order = 54)]
		public bool IsDerivedTableOrderBySupported { get; set; }

		/// <summary>
		/// Provider supports TAKE limit for UPDATE query.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 55)]
		public bool IsUpdateTakeSupported { get; set; }

		/// <summary>
		/// Provider supports SKIP+TAKE limit for UPDATE query.
		/// Default (set by <see cref="DataProviderBase"/>): <c>false</c>.
		/// </summary>
		[DataMember(Order = 56)]
		public bool IsUpdateSkipTakeSupported { get; set; }

		public bool GetAcceptsTakeAsParameterFlag(SelectQuery selectQuery)
		{
			return AcceptsTakeAsParameter || AcceptsTakeAsParameterIfSkip && selectQuery.Select.SkipValue != null;
		}

		public bool GetIsSkipSupportedFlag(ISqlExpression? takeExpression, ISqlExpression? skipExpression)
		{
			return IsSkipSupported || IsSkipSupportedIfTake && takeExpression != null;
		}

		public bool GetIsTakeHintsSupported(TakeHints hints)
		{
			if (TakeHintsSupported == null)
				return false;

			return (TakeHintsSupported.Value & hints) == hints;
		}

		#region Equality
		// equality support currently needed for remote context to avoid incorrect use of cached dependent types
		// with different flags
		// https://github.com/linq2db/linq2db/issues/1445
		public override int GetHashCode()
		{
			return IsParameterOrderDependent                           .GetHashCode()
				^ AcceptsTakeAsParameter                               .GetHashCode()
				^ AcceptsTakeAsParameterIfSkip                         .GetHashCode()
				^ IsTakeSupported                                      .GetHashCode()
				^ IsSkipSupported                                      .GetHashCode()
				^ IsSkipSupportedIfTake                                .GetHashCode()
				^ IsSubQueryTakeSupported                              .GetHashCode()
				^ IsJoinDerivedTableWithTakeInvalid                    .GetHashCode()
				^ IsCorrelatedSubQueryTakeSupported                    .GetHashCode()
				^ IsSupportsJoinWithoutCondition                       .GetHashCode()
				^ IsSubQuerySkipSupported                              .GetHashCode()
				^ IsSubQueryColumnSupported                            .GetHashCode()
				^ IsSubQueryOrderBySupported                           .GetHashCode()
				^ IsCountSubQuerySupported                             .GetHashCode()
				^ IsIdentityParameterRequired                          .GetHashCode()
				^ IsApplyJoinSupported                                 .GetHashCode()
				^ IsCrossApplyJoinSupportsCondition                    .GetHashCode()
				^ IsOuterApplyJoinSupportsCondition                    .GetHashCode()
				^ IsInsertOrUpdateSupported                            .GetHashCode()
				^ CanCombineParameters                                 .GetHashCode()
				^ MaxInListValuesCount                                 .GetHashCode()
				^ (TakeHintsSupported?                                 .GetHashCode() ?? 0)
				^ IsCrossJoinSupported                                 .GetHashCode()
				^ IsCommonTableExpressionsSupported                    .GetHashCode()
				^ IsOrderByAggregateFunctionsSupported                 .GetHashCode()
				^ IsAllSetOperationsSupported                          .GetHashCode()
				^ IsDistinctSetOperationsSupported                     .GetHashCode()
				^ IsCountDistinctSupported                             .GetHashCode()
				^ IsNestedJoinsSupported                               .GetHashCode()
				^ IsAggregationDistinctSupported                       .GetHashCode()
				^ IsUpdateFromSupported                                .GetHashCode()
				^ DefaultMultiQueryIsolationLevel                      .GetHashCode()
				^ AcceptsOuterExpressionInAggregate                    .GetHashCode()
				^ IsNamingQueryBlockSupported                          .GetHashCode()
				^ IsWindowFunctionsSupported                           .GetHashCode()
				^ RowConstructorSupport                                .GetHashCode()
				^ OutputDeleteUseSpecialTable                          .GetHashCode()
				^ OutputInsertUseSpecialTable                          .GetHashCode()
				^ OutputUpdateUseSpecialTables                         .GetHashCode()
				^ DoesNotSupportCorrelatedSubquery                     .GetHashCode()
				^ IsExistsPreferableForContains                        .GetHashCode()
				^ IsRowNumberWithoutOrderBySupported                   .GetHashCode()
				^ IsSubqueryWithParentReferenceInJoinConditionSupported.GetHashCode()
				^ IsColumnSubqueryWithParentReferenceSupported         .GetHashCode()
				^ IsColumnSubqueryWithParentReferenceAndTakeSupported  .GetHashCode()
				^ IsColumnSubqueryShouldNotContainParentIsNotNull      .GetHashCode()
				^ IsRecursiveCTEJoinWithConditionSupported             .GetHashCode()
				^ IsOuterJoinSupportsInnerJoin                         .GetHashCode()
				^ IsMultiTablesSupportsJoins                           .GetHashCode()
				^ IsCTESupportsOrdering                                .GetHashCode()
				^ IsAccessBuggyLeftJoinConstantNullability             .GetHashCode()
				^ SupportsBooleanComparison                            .GetHashCode()
				^ IsDerivedTableOrderBySupported                       .GetHashCode()
				^ IsUpdateTakeSupported                                .GetHashCode()
				^ IsUpdateSkipTakeSupported                            .GetHashCode()
				^ CustomFlags.Aggregate(0, (hash, flag) => flag.GetHashCode() ^ hash);
	}

		public override bool Equals(object? obj)
		{
			return obj is SqlProviderFlags other
				&& IsParameterOrderDependent                             == other.IsParameterOrderDependent
				&& AcceptsTakeAsParameter                                == other.AcceptsTakeAsParameter
				&& AcceptsTakeAsParameterIfSkip                          == other.AcceptsTakeAsParameterIfSkip
				&& IsTakeSupported                                       == other.IsTakeSupported
				&& IsSkipSupported                                       == other.IsSkipSupported
				&& IsSkipSupportedIfTake                                 == other.IsSkipSupportedIfTake
				&& IsSubQueryTakeSupported                               == other.IsSubQueryTakeSupported
				&& IsJoinDerivedTableWithTakeInvalid                     == other.IsJoinDerivedTableWithTakeInvalid
				&& IsCorrelatedSubQueryTakeSupported                     == other.IsCorrelatedSubQueryTakeSupported
				&& IsSupportsJoinWithoutCondition                        == other.IsSupportsJoinWithoutCondition
				&& IsSubQuerySkipSupported                               == other.IsSubQuerySkipSupported
				&& IsSubQueryColumnSupported                             == other.IsSubQueryColumnSupported
				&& IsSubQueryOrderBySupported                            == other.IsSubQueryOrderBySupported
				&& IsCountSubQuerySupported                              == other.IsCountSubQuerySupported
				&& IsIdentityParameterRequired                           == other.IsIdentityParameterRequired
				&& IsApplyJoinSupported                                  == other.IsApplyJoinSupported
				&& IsCrossApplyJoinSupportsCondition                     == other.IsCrossApplyJoinSupportsCondition
				&& IsOuterApplyJoinSupportsCondition                     == other.IsOuterApplyJoinSupportsCondition
				&& IsInsertOrUpdateSupported                             == other.IsInsertOrUpdateSupported
				&& CanCombineParameters                                  == other.CanCombineParameters
				&& MaxInListValuesCount                                  == other.MaxInListValuesCount
				&& TakeHintsSupported                                    == other.TakeHintsSupported
				&& IsCrossJoinSupported                                  == other.IsCrossJoinSupported
				&& IsCommonTableExpressionsSupported                     == other.IsCommonTableExpressionsSupported
				&& IsOrderByAggregateFunctionsSupported                  == other.IsOrderByAggregateFunctionsSupported
				&& IsAllSetOperationsSupported                           == other.IsAllSetOperationsSupported
				&& IsDistinctSetOperationsSupported                      == other.IsDistinctSetOperationsSupported
				&& IsCountDistinctSupported                              == other.IsCountDistinctSupported
				&& IsNestedJoinsSupported                                == other.IsNestedJoinsSupported
				&& IsAggregationDistinctSupported                        == other.IsAggregationDistinctSupported
				&& IsUpdateFromSupported                                 == other.IsUpdateFromSupported
				&& DefaultMultiQueryIsolationLevel                       == other.DefaultMultiQueryIsolationLevel
				&& AcceptsOuterExpressionInAggregate                     == other.AcceptsOuterExpressionInAggregate
				&& IsNamingQueryBlockSupported                           == other.IsNamingQueryBlockSupported
				&& IsWindowFunctionsSupported                            == other.IsWindowFunctionsSupported
				&& RowConstructorSupport                                 == other.RowConstructorSupport
				&& OutputDeleteUseSpecialTable                           == other.OutputDeleteUseSpecialTable
				&& OutputInsertUseSpecialTable                           == other.OutputInsertUseSpecialTable
				&& OutputUpdateUseSpecialTables                          == other.OutputUpdateUseSpecialTables
				&& DoesNotSupportCorrelatedSubquery                      == other.DoesNotSupportCorrelatedSubquery
				&& IsExistsPreferableForContains                         == other.IsExistsPreferableForContains
				&& IsRowNumberWithoutOrderBySupported                    == other.IsRowNumberWithoutOrderBySupported
				&& IsSubqueryWithParentReferenceInJoinConditionSupported == other.IsSubqueryWithParentReferenceInJoinConditionSupported
				&& IsColumnSubqueryWithParentReferenceSupported          == other.IsColumnSubqueryWithParentReferenceSupported
				&& IsColumnSubqueryWithParentReferenceAndTakeSupported   == other.IsColumnSubqueryWithParentReferenceAndTakeSupported
				&& IsColumnSubqueryShouldNotContainParentIsNotNull       == other.IsColumnSubqueryShouldNotContainParentIsNotNull
				&& IsRecursiveCTEJoinWithConditionSupported              == other.IsRecursiveCTEJoinWithConditionSupported
				&& IsOuterJoinSupportsInnerJoin                          == other.IsOuterJoinSupportsInnerJoin
				&& IsMultiTablesSupportsJoins                            == other.IsMultiTablesSupportsJoins
				&& IsCTESupportsOrdering                                 == other.IsCTESupportsOrdering
				&& IsAccessBuggyLeftJoinConstantNullability              == other.IsAccessBuggyLeftJoinConstantNullability
				&& SupportsBooleanComparison                             == other.SupportsBooleanComparison
				&& IsDerivedTableOrderBySupported                        == other.IsDerivedTableOrderBySupported
				&& IsUpdateTakeSupported                                 == other.IsUpdateTakeSupported
				&& IsUpdateSkipTakeSupported                             == other.IsUpdateSkipTakeSupported
				// CustomFlags as List wasn't best idea
				&& CustomFlags.Count                                     == other.CustomFlags.Count
				&& (CustomFlags.Count                                    == 0
					|| CustomFlags.OrderBy(_ => _).SequenceEqual(other.CustomFlags.OrderBy(_ => _)));
		}
		#endregion
	}

}
