namespace Symmetry.Test
{
	using System;
	using System.Linq;
	using NUnit.Framework;

	[TestFixture()]
	public class TestOption
	{
		[Test()]
		public void TestConstructors () {
			
			Option<string> a = Option.Some("Hello");
			Option<string> b = Option.Create("Hello");
			Option<string> c = Option.Create<string>(null);
			Option<string> d = Option<string>.None();
			Option<string> e = "Hello";
			Option<string> f = Option.None;
			
			
			Assert.IsTrue(a.IsSome());
			Assert.IsTrue(b.IsSome());
			Assert.IsFalse(c.IsSome());
			Assert.IsFalse(d.IsSome());
			Assert.IsTrue(e.IsSome());
			Assert.IsFalse(f.IsSome());
		}
		
		[Test()]
		public void TestEnumerable () {
		
			Option<int> none = Option.None;
			
			Assert.AreEqual(0, none.Count()); 
			Assert.AreEqual(0, none.Aggregate(0, (sum, x) => sum + x));
			
			var some = Option.Some<int>(3);
			
			Assert.AreEqual(1, some.Count()); 
			Assert.AreEqual(3, some.Aggregate(0, (sum, x) => sum + x));
		}
	}
}

