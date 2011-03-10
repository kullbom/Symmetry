using System;
using NUnit.Framework;

namespace Symmetry.Test
{
	[TestFixture()]
	public class TestUnit
	{
		[Test()]
		public void TestEquality ()
		{
			var unit1 = Unit.The;
			var unit2 = Unit.The;
			
			Assert.IsTrue(unit1.Equals(unit2));
			Assert.IsTrue(unit2.Equals(unit1));
		}
	}
}

