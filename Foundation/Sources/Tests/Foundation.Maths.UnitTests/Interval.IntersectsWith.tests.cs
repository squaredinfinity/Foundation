using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths
{
    [TestClass]
    public class Interval__IntersectsWith
    {
        [TestMethod]
        public void EmptyInterval__ReturnsFalse()
        {
            var a = new Interval(-10, 10);
            var b = Interval.Empty;

            Assert.IsFalse(a.IntersectsWith(b));
            Assert.IsFalse(b.IntersectsWith(a));
            Assert.IsFalse(b.IntersectsWith(b));
        }

        [TestMethod]
        public void NonEmpty_NotContainedInterval__ReturnsFalse()
        {
            // (-10; 10)
            var a = new Interval(-10, 10);
            // (-100; -10)
            var b = new Interval(-100, -10);

            Assert.IsFalse(a.IntersectsWith(b));
            Assert.IsFalse(b.IntersectsWith(a));

            // [-10; 10)
            a = new Interval(-10, true, 10, false);
            // (-100; -10)
            b = new Interval(-100, -10);

            Assert.IsFalse(a.IntersectsWith(b));
            Assert.IsFalse(b.IntersectsWith(a));

            // (-10; 10)
            a = new Interval(-10, 10);
            // (-100; -10]
            b = new Interval(-100, false, -10, true);

            Assert.IsFalse(a.IntersectsWith(b));
            Assert.IsFalse(b.IntersectsWith(a));
        }

        [TestMethod]
        public void NonEmpty_ContainedInterval__ReturnsTrue()
        {
            // (-10; 10)
            var a = new Interval(-10, 10);
            // (-11; -9)
            var b = new Interval(-11, -9);

            Assert.IsTrue(a.IntersectsWith(b));
            Assert.IsTrue(b.IntersectsWith(a));

            // [-10; 10)
            a = new Interval(-10, true, 10, false);
            // (-100; -10]
            b = new Interval(-100, false, -10, true);

            Assert.IsTrue(a.IntersectsWith(b));
            Assert.IsTrue(b.IntersectsWith(a));
        }
    }
}
