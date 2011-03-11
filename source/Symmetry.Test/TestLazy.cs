using System;
using NUnit.Framework;

namespace Symmetry.Test
{
	[TestFixture()]
	public class TestLazy
	{
		[Test()]
		public void TestConsistancy ()
		{
			var state = 0;
			var cell = Lazy.Create(() => { state++; return state; });
			
			Assert.AreEqual(cell.Force(), cell.Force());
			Assert.AreEqual(cell.Force(), cell.Force());
			Assert.AreEqual(cell.Force(), cell.Force());
		}
	}
}

