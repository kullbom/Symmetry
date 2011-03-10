namespace Symmetry.Test
{
	using System;
	using NUnit.Framework;

	[TestFixture()]
	public class TestUnion
	{
		[Test()]
		public void TestConstructors ()
		{
			Union<int, string> a = "Hello";
			Union<int, string> b = 11;
			
			Assert.AreEqual("Hello", a.Match<string>(l => l.ToString(), r => r));  
			Assert.AreEqual(11, b.Match<int>(l => l, r => 0));  
		}
 	}
}

