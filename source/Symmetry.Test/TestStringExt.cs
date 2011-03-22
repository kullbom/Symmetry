using System;
using NUnit.Framework;

namespace Symmetry.Test
{
	[TestFixture()]
	public class TestStringExt
	{
		[Test()]
		public void TestContent ()
		{
            //var sb = new StrBuilder();
            //var sb2 = sb.Append("Hej");
            //var sb3 = sb2.Append("Då");
			
            //Assert.AreEqual("", sb.GetString());
            //Assert.AreEqual("Hej", sb2.GetString());
            //Assert.AreEqual("HejDå", sb3.GetString());
		}

        private int nAppends = 1000000;

        [Test()]
        public void TestStrLstPerformance()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var sb = Lst.Empty<string>();
            for (int i = 0; i < this.nAppends; i++)
                sb = Lst.Cons("Foo", sb);

            var strBuilderAppendTime = watch.ElapsedMilliseconds;

            var strBuilderResult = sb.Concat();

            var strBuilderResultTime = watch.ElapsedMilliseconds;
        }
        
        [Test()]
        public void TestStringBuilderPerformance()
        {
            var watch = new System.Diagnostics.Stopwatch();
			watch.Start();
            
            var sbNet = new System.Text.StringBuilder();
            for (int i = 0; i < this.nAppends; i++)
				sbNet.Append("Foo");
			
			var stringBuilderAppendTime = watch.ElapsedMilliseconds;
			
			var stringBuilderResult = sbNet.ToString();
			
			var stringBuilderResultTime = watch.ElapsedMilliseconds;
        }   
	}
}

