﻿using System;
using System.Linq;

using LinqToDB;
using LinqToDB.DataProvider.SqlCe;

using NUnit.Framework;

namespace Tests.Linq
{
	[TestFixture]
	public class QueryExtensionSqlCeTests : TestBase
	{
		[Test]
		public void TableHintTest(
			[IncludeDataSources(true, ProviderName.SqlCe)] string context,
			[Values(
				Hints.TableHint.HoldLock,
				Hints.TableHint.NoLock,
				Hints.TableHint.PagLock,
				Hints.TableHint.RowLock,
				Hints.TableHint.TabLock,
				Hints.TableHint.UpdLock,
				Hints.TableHint.XLock
				)] string hint)
		{
			using var db = GetDataContext(context);

			var q =
				from p in db.Parent.With(hint)
				select p;

			_ = q.ToList();

			Assert.That(LastQuery, Contains.Substring($"WITH ({hint})"));
		}

		[Test]
		public void TableHintIndexTest([IncludeDataSources(true, ProviderName.SqlCe)] string context)
		{
			using var db = GetDataContext(context);

			var q =
				from p in db.Person
					.TableHint("Index", "PK_Person")
					.With(Hints.TableHint.NoLock)
				select p;

			_ = q.ToList();

			Assert.That(LastQuery, Contains.Substring("WITH (Index(PK_Person), NoLock)"));
		}

		[Test]
		public void TablesInScopeHintTest(
			[IncludeDataSources(true, ProviderName.SqlCe)] string context)
		{
			using var db = GetDataContext(context);

			var q =
			(
				from p in db.Person.With(Hints.TableHint.Index("PK_Person"))
				from c in db.Child
				where c.ParentID == p.ID
				select p
			)
			.TablesInScopeHint(Hints.TableHint.NoLock);

			q =
			(
				from p in q
				from c in db.Child
				from p1 in db.Parent.TablesInScopeHint(Hints.TableHint.HoldLock)
				where c.ParentID == p.ID && c.Parent!.ParentID > 0 && p1.ParentID == p.ID
				select p
			)
			.TablesInScopeHint(Hints.TableHint.PagLock);

			q =
				from p in q
				from c in db.Child
				where c.ParentID == p.ID
				select p;

			_ = q.ToList();

			var test = LastQuery?.Replace("\r", "");

			Assert.That(test, Contains.Substring("[Person] [p] WITH (Index(PK_Person), NoLock)"));
			Assert.That(test, Contains.Substring("[Child] [c_1] WITH (NoLock)"));
			Assert.That(test, Contains.Substring("[Parent] [p1] WITH (HoldLock)"));
			Assert.That(test, Contains.Substring("[Child] [c_2] WITH (PagLock)"));
			Assert.That(test, Contains.Substring("[Parent] [a_Parent] WITH (PagLock)"));
		}

		[Test]
		public void InsertTest([IncludeDataSources(true, ProviderName.SqlCe)] string context)
		{
			using var db = GetDataContext(context);

			(
				from c in db.Child.TableHint(Hints.TableHint.NoLock)
				where c.ParentID < -1111
				select c
			)
			.Insert(db.Child, c => new()
			{
				ChildID = c.ChildID * 2
			});

			Assert.That(LastQuery, Contains.Substring("WITH (NoLock)"));
		}

		[Test]
		public void UnionTest([IncludeDataSources(true, ProviderName.SqlCe)] string context)
		{
			using var db = GetDataContext(context);

			var q1 =
				from c in db.Child
				join p in db.Parent.TableHint(Hints.TableHint.NoLock) on c.ParentID equals p.ParentID
				select p;

			var q =
				q1.QueryName("qb_1").Union(q1.QueryName("qb_2"));

			_ = q.ToList();

			Assert.That(LastQuery, Contains.Substring("[p] WITH (NoLock)"));
			Assert.That(LastQuery, Contains.Substring("[p_1] WITH (NoLock)"));
		}
	}
}
