using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics.Distributions
{
    [TestClass]
    public partial class TriangularDistribution__CDF
    {
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
