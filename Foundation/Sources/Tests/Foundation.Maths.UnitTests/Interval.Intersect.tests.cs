using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths
{
    [TestClass]
    public class Interval__Intersect
    {
        [TestMethod]
        public void EmptyInterval__ReturnsEmptyInterval()
        {
            var a = new Interval(-10, 10);
            var b = Interval.Empty;

            Assert.IsTrue(Interval.Intersect(a, b).IsEmpty);
            Assert.IsTrue(Interval.Intersect(b, a).IsEmpty);
        }

        [TestMethod]
        public void Self__ReturnsSelf()
        {
            var a = new Interval(-10, 10);

            Assert.AreEqual(a, a.Intersect(a));
        }

        [TestMethod]
        public void NonEmpty_NotIntersectingInterval__ReturnsEmptyInterval()
        {
            // (-10; 10)
            var a = new Interval(-10, false, 10, false);
            // (-100; -10)
            var b = new Interval(-100, false, -10, false);

            Assert.IsTrue(a.Intersect(b).IsEmpty);
            Assert.IsTrue(b.Intersect(a).IsEmpty);

            // [-10; 10)
            a = new Interval(-10, true, 10, false);
            // (-100; -10)
            b = new Interval(-100, false, -10, false);

            Assert.IsTrue(a.Intersect(b).IsEmpty);
            Assert.IsTrue(b.Intersect(a).IsEmpty);

            // (-10; 10)
            a = new Interval(-10, false, 10, false);
            // (-100; -10]
            b = new Interval(-100, false, -10, true);

            Assert.IsTrue(a.Intersect(b).IsEmpty);
            Assert.IsTrue(b.Intersect(a).IsEmpty);
        }

        [TestMethod]
        public void NonEmpty_IntersectingInterval__ReturnsIntersection()
        {
            // (-10; 10)
            var a = new Interval(-10, 10);
            // (-11; -9)
            var b = new Interval(-11, -9);

            Assert.AreEqual(new Interval(-10, -9), a.Intersect(b));
            Assert.AreEqual(new Interval(-10, -9), b.Intersect(a));

            // [-10; 10)
            a = new Interval(-10, true, 10, false);
            // (-100; -10]
            b = new Interval(-100, false, -10, true);

            Assert.AreEqual(new Interval(-10, true, -10, true), a.Intersect(b));
            Assert.AreEqual(new Interval(-10, true, -10, true), b.Intersect(a));
        }
    }
}
