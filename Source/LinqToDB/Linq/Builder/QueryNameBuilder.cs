﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToDB.Linq.Builder
{
	using Extensions;
	using LinqToDB.Expressions;

	sealed class QueryNameBuilder : MethodCallBuilder
	{
		protected override bool CanBuildMethodCall(ExpressionBuilder builder, MethodCallExpression methodCall, BuildInfo buildInfo)
		{
			return methodCall.IsQueryable(new[] { nameof(LinqExtensions.QueryName) });
		}

		protected override BuildSequenceResult BuildMethodCall(ExpressionBuilder builder, MethodCallExpression methodCall, BuildInfo buildInfo)
		{
			var sequence    = builder.BuildSequence(new(buildInfo, methodCall.Arguments[0]));

			sequence.SelectQuery.QueryName = (string?)builder.EvaluateExpression(methodCall.Arguments[1]);
			sequence = new SubQueryContext(sequence);

			return BuildSequenceResult.FromContext(sequence);
		}
	}
}
