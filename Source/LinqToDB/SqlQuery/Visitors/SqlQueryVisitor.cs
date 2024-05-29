﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LinqToDB.SqlQuery.Visitors
{
	using Common;
	using SqlQuery;

	/// <summary>
	/// This base visitor implements:
	/// <list type="bullet">
	/// <item>tracking of replaced elemnents with <see cref="GetReplacement"/> API to access replacements;</item>
	/// <item>changes element's <see cref="VisitMode.Transform"/> to <see cref="VisitMode.Modify"/> for already replaced element;</item>
	/// <item>provides <see cref="ProcessElement"/> API to re-visit element;</item>
	/// <item>skips visit of replaced element.</item>
	/// </list>
	/// </summary>
	public abstract class SqlQueryVisitor : QueryElementVisitor
	{
		IVisitorTransformationInfo? _transformationInfo;

		public interface IVisitorTransformationInfo
		{
			bool GetReplacement(IQueryElement   element, [NotNullWhen(true)] out IQueryElement? replacement);
			bool IsReplaced(IQueryElement       element);
			void RegisterReplaced(IQueryElement newElement, IQueryElement oldElement);

			int         Version { get; }
			public void GetReplacements(Dictionary<IQueryElement, IQueryElement> objectTree);
		}

		protected SqlQueryVisitor(VisitMode visitMode, IVisitorTransformationInfo? transformationInfo) : base(visitMode)
		{
			SetTransformationInfo(transformationInfo);
		}

		/// <summary>
		/// Resets visitor to initial state.
		/// </summary>
		public virtual void Cleanup()
		{
			_transformationInfo = null;
		}

		protected void SetTransformationInfo(IVisitorTransformationInfo? transformationInfo)
		{
			_transformationInfo = transformationInfo;
		}

		[return: NotNullIfNotNull(nameof(element))]
		public override IQueryElement? Visit(IQueryElement? element)
		{
			if (element == null) return null;

			if (GetReplacement(element, out var newElement))
				return newElement;

			return base.Visit(element);
		}

		public override VisitMode GetVisitMode(IQueryElement element)
		{
			var visitMode = VisitMode;
			if (visitMode == VisitMode.ReadOnly)
				return VisitMode.ReadOnly;

			// when element was already replaced with new instance, we don't need to replace it again and can modify it inplace
			if (visitMode == VisitMode.Transform && _transformationInfo?.IsReplaced(element) == true)
				return VisitMode.Modify;

			return visitMode;
		}

		/// <summary>
		/// Visits <paramref name="element"/> and correct it, if it contains old replaced elements.
		/// </summary>
		public virtual IQueryElement ProcessElement(IQueryElement element)
		{
			var version = _transformationInfo?.Version ?? -1;

			var newElement = Visit(element);

			if (VisitMode == VisitMode.ReadOnly && !ReferenceEquals(newElement, element))
				throw new InvalidOperationException("VisitMode is readonly but element changed.");

			// Execute replacer to correct elements
			if ((_transformationInfo?.Version ?? -1) != version && (VisitMode == VisitMode.Modify || VisitMode == VisitMode.Transform && !ReferenceEquals(newElement, element)))
			{
				if (_transformationInfo != null)
				{
					// go through tree and correct references
					var replacer  = new Replacer(this);
					var finalized = replacer.Visit(newElement);
					if (!ReferenceEquals(newElement, finalized))
					{
						throw new InvalidOperationException($"Visitor replaced already replaced element {newElement}");
					}

					return finalized;
				}
			}

			return newElement;
		}

		protected override IQueryElement VisitCteClauseReference(CteClause element)
		{
			if (GetReplacement(element, out var newElement))
				return newElement;

			return base.VisitCteClauseReference(element);
		}

		/// <summary>
		/// Remembers element replacement.
		/// </summary>
		public override IQueryElement NotifyReplaced(IQueryElement newElement, IQueryElement oldElement)
		{
			_transformationInfo ??= new VisitorTransformationInfo();
			_transformationInfo.RegisterReplaced(newElement, oldElement);

			return base.NotifyReplaced(newElement, oldElement);
		}

		/// <summary>
		/// Adds explicit replacement map.
		/// </summary>
		protected void AddReplacements(IReadOnlyDictionary<IQueryElement, IQueryElement> replacements)
		{
			_transformationInfo ??= new VisitorTransformationInfo();

			foreach (var pair in replacements)
			{
				if (ReferenceEquals(pair.Key, pair.Value))
					throw new ArgumentException($"{nameof(replacements)} contains entry with key == value");

				_transformationInfo.RegisterReplaced(pair.Value, pair.Key);
			}
		}

		/// <summary>
		/// Returns replacement element for <paramref name="element"/> if it was registered as replaced.
		/// </summary>
		protected bool GetReplacement(IQueryElement element, [NotNullWhen(true)] out IQueryElement? replacement)
		{
			if (_transformationInfo == null)
			{
				replacement = null;
				return false;
			}

			return _transformationInfo.GetReplacement(element, out replacement);
		}

		/// <summary>
		/// Writes registered replacement pairs to <paramref name="objectTree"/> dictionary.
		/// </summary>
		public void GetReplacements(Dictionary<IQueryElement, IQueryElement> objectTree)
		{
			_transformationInfo?.GetReplacements(objectTree);
		}

		/// <summary>
		/// Visitor replaces elements in visited tree with new elements from <see cref="_transformationInfo"/> replacement map.
		/// Separate replace-only visitor used to avoid side-effects from parent <see cref="SqlQueryVisitor"/> implementor.
		/// </summary>
		sealed class Replacer : QueryElementVisitor
		{
			readonly SqlQueryVisitor _queryVisitor;

			public Replacer(SqlQueryVisitor queryVisitor) : base(queryVisitor.VisitMode)
			{
				_queryVisitor = queryVisitor;
			}

			public override IQueryElement NotifyReplaced(IQueryElement newElement, IQueryElement oldElement)
			{
				return _queryVisitor.NotifyReplaced(newElement, oldElement);
			}

			public override VisitMode GetVisitMode(IQueryElement element)
			{
				var visitMode = VisitMode;

				if (visitMode == VisitMode.ReadOnly)
					return VisitMode.ReadOnly;

				// when element was already replaced with new instance, we don't need to replace it again and can modify it inplace
				if (visitMode == VisitMode.Transform && _queryVisitor._transformationInfo?.IsReplaced(element) == true)
					return VisitMode.Modify;

				return visitMode;
			}

			[return: NotNullIfNotNull(nameof(element))]
			public override IQueryElement? Visit(IQueryElement? element)
			{
				if (element != null && _queryVisitor.GetReplacement(element, out var newElement))
					return newElement;

				return base.Visit(element);
			}

			// CteClause reference not visited by main dispatcher
			protected override IQueryElement VisitCteClauseReference(CteClause element)
			{
				if (_queryVisitor.GetReplacement(element, out var newElement))
					return newElement;

				return base.VisitCteClauseReference(element);
			}
		}

		public class VisitorTransformationInfo : IVisitorTransformationInfo
		{
			Dictionary<IQueryElement, IQueryElement>? _replacements;
			HashSet<IQueryElement>?                   _replaced;
			int                                       _version;

			public bool GetReplacement(IQueryElement element, [NotNullWhen(true)] out IQueryElement? replacement)
			{
				replacement = null;

				while (_replacements?.TryGetValue(element, out var current) == true)
				{
					if (ReferenceEquals(element, current))
					{
						// Self replacements stops visitor to go deeper
						replacement = current;
						break;
					}
					replacement = element = current;
				}

				return replacement != null;
			}

			public bool IsReplaced(IQueryElement element)
			{
				return _replaced?.Contains(element) == true;
			}

			public void RegisterReplaced(IQueryElement newElement, IQueryElement oldElement)
			{
				_replacements ??= new Dictionary<IQueryElement, IQueryElement>(Utils.ObjectReferenceEqualityComparer<IQueryElement>.Default);
				_replaced     ??= new HashSet<IQueryElement>(Utils.ObjectReferenceEqualityComparer<IQueryElement>.Default);

				_replacements[oldElement] = newElement;
				_replaced.Add(newElement);
				_version++;
			}

			public int Version => _version;
			public void GetReplacements(Dictionary<IQueryElement, IQueryElement> objectTree)
			{
				if (_replacements != null)
				{
					foreach (var pair in _replacements)
					{
						if (ReferenceEquals(pair.Key, pair.Value))
							throw new ArgumentException($"{nameof(objectTree)} contains entry with key == value");

						objectTree[pair.Key] = pair.Value;
					}
				}
			}
		}

	}
}
