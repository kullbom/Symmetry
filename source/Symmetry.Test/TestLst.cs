namespace Symmetry.Test
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;

	[TestFixture()]
	public class TestLst
	{
		[Test()]
		public void TestContent ()
		{
			var lst1 = Lst.Create(1,2,3,4,5,6);
			var odds = lst1.Filter(x => x%2 == 0);
			
			Assert.AreEqual(6, lst1.Count());
			Assert.AreEqual(21, lst1.Aggregate(0, (sum, x) => sum + x));
			Assert.AreEqual(12, odds.Sum());
		}
		
		[Test()]
		public void TestEquality ()
		{
			var l0 = Lst.Create(1,2,3,4,5,6);
			var l1 = Lst.Create(1,2,3,4,5,6);
			var l2 = Lst.Create(1,2,3,4,5,7);
			var l3 = Lst.Create(1,2,3,4,5);
			var l4 = Lst.Create(5,4,3,2,1);
			
			Assert.IsTrue(l0.IsEqual(l1));
			Assert.IsFalse(l0.IsEqual(l2));
			Assert.IsFalse(l0.IsEqual(l3));
			Assert.IsFalse(l0.IsEqual(l4));
			Assert.IsFalse(l3.IsEqual(l4));
			
			// And if all elements are considered equal... - only length should matter now.
			Assert.IsTrue(l0.IsEqual(l1, (x, y) => true));
			Assert.IsTrue(l0.IsEqual(l2, (x, y) => true));
			Assert.IsFalse(l0.IsEqual(l3, (x, y) => true));
			Assert.IsFalse(l0.IsEqual(l4, (x, y) => true));
			Assert.IsTrue(l3.IsEqual(l4, (x, y) => true));
		}
		
		[Test()]
		public void TestFolds ()
		{
			var lst = Lst.Create(1,2,3,4,5,6);
			
			// (((((0 - 1) - 2) - 3) - 4) - 5) - 6
			Assert.AreEqual(-21, lst.FoldL(0, (seed, e) => seed - e));
			// (((((0 - 6) - 5) - 4) - 3) - 2) - 1
			Assert.AreEqual(-21, lst.FoldR(0, (seed, e) => seed - e));

			// 6 - (5 - (4 - (3 - (2 - (1 - 0)))))
			Assert.AreEqual(3, lst.FoldL(0, (seed, e) => e - seed));
			// 1 - (2 - (3 - (4 - (5 - (6 - 0)))))
			Assert.AreEqual(-3, lst.FoldR(0, (seed, e) => e - seed));
		}
	}
}

