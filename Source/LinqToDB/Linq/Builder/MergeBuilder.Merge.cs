﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToDB.Linq.Builder
{
	using LinqToDB.Expressions;
	using SqlQuery;
	using Common;

	using static LinqToDB.Reflection.Methods.LinqToDB.Merge;

	internal partial class MergeBuilder
	{
		internal sealed class Merge : MethodCallBuilder
		{
			static readonly MethodInfo[] _supportedMethods = {MergeMethodInfo1, MergeMethodInfo2};

			protected override bool CanBuildMethodCall(ExpressionBuilder builder, MethodCallExpression methodCall, BuildInfo buildInfo)
			{
				return methodCall.IsSameGenericMethod(_supportedMethods);
			}

			protected override BuildSequenceResult BuildMethodCall(ExpressionBuilder builder, MethodCallExpression methodCall, BuildInfo buildInfo)
			{
				// Merge(ITable<TTarget> target, string hint)

				var disableFilters = methodCall.Arguments[0] is not MethodCallExpression mc || mc.Method.Name != nameof(LinqExtensions.AsCte);
				if (disableFilters)
					builder.PushDisabledQueryFilters([]);

				var target = builder.BuildSequence(new BuildInfo(buildInfo, methodCall.Arguments[0], new SelectQuery()) { AssociationsAsSubQueries = true });

				if (disableFilters)
					builder.PopDisabledFilter();

				var targetTable = GetTargetTable(target);

				if (targetTable == null)
					throw new NotImplementedException("Currently, only CTEs are supported as the target of a merge. You can fix by calling .AsCte() before calling .Merge()");

				var merge = new SqlMergeStatement(targetTable);
				if (methodCall.Arguments.Count == 2)
					merge.Hint = builder.EvaluateExpression<string>(methodCall.Arguments[1]);

				target.SetAlias(merge.Target.Alias!);

				return BuildSequenceResult.FromContext(new MergeContext(merge, target));
			}
		}
	}
}
