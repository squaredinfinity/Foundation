using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics.Distributions
{
    [TestClass]
    public partial class TriangularDistribution__CDF
    {
        //[TestMethod]
        //public void XXXX()
        //{
        //    var ud = new UniformDistribution(0, 100);

        //    var sw = Stopwatch.StartNew();

        //    for(int i = 0; i < 1000000; i++)
        //    {
        //        var x = ud.CDF(47.5);
        //    }

        //    sw.Stop();
        //    Trace.WriteLine("UD: " + sw.Elapsed.TotalMilliseconds);


        //    var td = new TriangularDistribution(0, 100, 50);

        //    sw = Stopwatch.StartNew();

        //    for (int i = 0; i < 1000000; i++)
        //    {
        //        var x = td.CDF(47.5);
        //    }

        //    sw.Stop();
        //    Trace.WriteLine("TD: " + sw.Elapsed.TotalMilliseconds);
        //}

        [TestMethod]
        public void x_equals_a__returns_0()
        {
            var a = 10;
            var b = 30;
            var c = 15;
            var d = new TriangularDistribution(a, b, c);

            Assert.AreEqual(0, d.CDF(a));
        }

        [TestMethod]
        public void x_less_than_a__returns_0()
        {
            var a = 10;
            var b = 30;
            var c = 15;
            var d = new TriangularDistribution(a, b, c);

            Assert.AreEqual(0, d.CDF(a - 1));
        }

        [TestMethod]
        public void b_equals_x__returns_1()
        {
            var a = 10;
            var b = 30;
            var c = 15;
            var d = new TriangularDistribution(a, b, c);

            Assert.AreEqual(1, d.CDF(b));
        }

        [TestMethod]
        public void b_less_than_x__returns_1()
        {
            var a = 10;
            var b = 30;
            var c = 15;
            var d = new TriangularDistribution(a, b, c);

            Assert.AreEqual(1, d.CDF(b + 1));
        }

        [TestMethod]
        public void within_0_1_range()
        {
            var a = 10;
            var b = 30;
            var c = 15;
            var d = new TriangularDistribution(a, b, c);

            for(int i = a; i < b; i++)
            {
                var cd = d.CDF(i);

                Assert.IsTrue(cd.IsGreaterThanOrClose(0.0));
                Assert.IsTrue(cd.IsLessThanOrClose(1.0));
            }
        }
    }
}
