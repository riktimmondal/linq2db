﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToDB.Linq.Builder
{
	using LinqToDB.Expressions;
	using Mapping;
	using SqlQuery;

	internal sealed class CteTableContext: BuildContextBase, ITableContext
	{
		readonly Type          _objectType;

		public override  MappingSchema MappingSchema => CteContext.MappingSchema;

		public CteContext  CteContext { get; set; }

		public SqlCteTable CteTable
		{
			get
			{
				if (_cteTable == null!)
				{
					if (CteContext != null!)
						_cteTable = new SqlCteTable(CteContext.CteClause, ObjectType);
					else
						throw new InvalidOperationException("CteContext not initialized");
				}

				return _cteTable!;
			}
			set => _cteTable = value;
		}

		public Type          ObjectType   => _objectType;
		public SqlTable      SqlTable     => CteTable;
		public LoadWithInfo  LoadWithRoot { get; set; } = new();
		public MemberInfo[]? LoadWithPath { get; set; }

		public CteTableContext(ExpressionBuilder builder, IBuildContext? parent, Type objectType, SelectQuery selectQuery, CteContext cteContext, bool isTest)
			: this(builder, parent, objectType, selectQuery)
		{
			_objectType = objectType;
			Parent      = parent;

			CteContext = cteContext;
			CteTable   = new SqlCteTable(CteContext.CteClause, _objectType);

			if (!isTest)
				SelectQuery.From.Table(CteTable);
		}

		CteTableContext(ExpressionBuilder builder, IBuildContext? parent, Type objectType, SelectQuery selectQuery)
			: base(builder, objectType, selectQuery)
		{
			_objectType = objectType;
			Parent      = parent;
			CteTable    = default!;
			CteContext  = default!;
		}

		public override void SetRunQuery<T>(Query<T> query, Expression expr)
		{
			var mapper = Builder.BuildMapper<T>(SelectQuery, expr);

			QueryRunner.SetRunQuery(query, mapper);
		}

		public override IBuildContext? GetContext(Expression expression, BuildInfo buildInfo)
		{
			if (!buildInfo.CreateSubQuery)
				return this;

			var expr = Builder.GetSequenceExpression(this);
			if (expr == null)
				return this;

			var context = Builder.BuildSequence(new BuildInfo(buildInfo, expr));

			return context;
		}

		readonly Dictionary<Expression, SqlPlaceholderExpression> _fieldsMap = new (ExpressionEqualityComparer.Instance);

		SqlCteTable? _cteTable;

		public override Expression MakeExpression(Expression path, ProjectFlags flags)
		{
			if (flags.HasFlag(ProjectFlags.Root) || flags.HasFlag(ProjectFlags.AssociationRoot) || flags.HasFlag(ProjectFlags.Table))
				return path;

			if (flags.IsExtractProjection())
			{
				CteContext.InitQuery();
				return path;
			}

			var ctePath = SequenceHelper.CorrectExpression(path, this, CteContext);
			if (!ReferenceEquals(ctePath, path))
			{
				CteContext.InitQuery();

				var translated = Builder.BuildSqlExpression(CteContext, ctePath, flags.SqlFlag(), buildFlags: ExpressionBuilder.BuildFlags.ForceAssignments);

				if (!flags.HasFlag(ProjectFlags.Test))
				{
					translated = SequenceHelper.ReplaceContext(translated, CteContext, this);

					// replace tracking path back
					translated = SequenceHelper.CorrectTrackingPath(translated, CteContext, this);

					var placeholders = ExpressionBuilder.CollectPlaceholders(translated);

					translated = RemapFields(translated, placeholders);
				}

				return translated;
			}

			return ctePath;
		}

		Expression RemapFields(Expression expression, List<SqlPlaceholderExpression> placeholders)
		{
			if (placeholders.Count == 0)
				return expression;

			var newPlaceholders = new SqlPlaceholderExpression[placeholders.Count];

			for (var index = 0; index < placeholders.Count; index++)
			{
				var placeholder = placeholders[index];

				if (placeholder.TrackingPath == null)
					continue;

				if (!_fieldsMap.TryGetValue(placeholder.TrackingPath, out var newPlaceholder))
				{
					var field = QueryHelper.GetUnderlyingField(placeholder.Sql);

					if (field == null)
						throw new InvalidOperationException($"Could not get field from SQL: {placeholder.Sql}");

					var newField = new SqlField(field);

					CteTable.Add(newField);

					newPlaceholder = ExpressionBuilder.CreatePlaceholder(SelectQuery, newField,
						placeholder.TrackingPath,
						index: placeholder.Index);

					newPlaceholder = newPlaceholder.WithType(placeholder.Type);

					_fieldsMap[placeholder.TrackingPath] = newPlaceholder;
				}
				else
				{
					newPlaceholder = newPlaceholder.WithType(placeholder.Type);
				}

				newPlaceholders[index] = newPlaceholder;
			}

			var transformed = expression.Transform((placeholders, newPlaceholders), (ctx, e) =>
			{
				if (e.NodeType == ExpressionType.Extension && e is SqlPlaceholderExpression placeholder)
				{
					var index = ctx.placeholders.IndexOf(placeholder);
					if (index >= 0)
					{
						return ctx.newPlaceholders[index];
					}
				}

				return e;
			});

			return transformed;
		}

		public override IBuildContext Clone(CloningContext context)
		{
			var newContext = new CteTableContext(Builder, Parent, ObjectType, context.CloneElement(SelectQuery));
			context.RegisterCloned(this, newContext);
			newContext.CteContext = context.CloneContext(CteContext);
			return newContext;
		}

		public override SqlStatement GetResultStatement()
		{
			return new SqlSelectStatement(SelectQuery);
		}
	}
}
