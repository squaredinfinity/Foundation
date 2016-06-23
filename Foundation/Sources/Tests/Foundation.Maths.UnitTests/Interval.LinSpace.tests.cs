using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths
{
    [TestClass]
    public class Interval__LinSpace
    {
        [TestMethod]
        public void by_default__returns_100_points()
        {
            // (0;100]
            var points = new Interval(0, false, 100, true).LinSpace();

            Assert.AreEqual(100, points.Length);
        }

        [TestMethod]
        public void from_closed__includes_from()
        {
            // [1;100]
            var points = new Interval(1, 100).LinSpace();
            
            Assert.AreEqual(1, points[0]);
        }

        [TestMethod]
        public void to_closed__includes_to()
        {
            // [1;100]
            var points = new Interval(1, 100).LinSpace();

            Assert.AreEqual(100, points[99]);
        }

        [TestMethod]
        public void closed_interval__generates_valid_points()
        {
            // [1;100]
            var points = new Interval(1, 100).LinSpace();

            for (int i = 1; i <= 100; i++)
                Assert.AreEqual(i, points[i - 1]);
        }

        [TestMethod]
        public void from_open__excludes_from()
        {
            // (0;100]
            var points = new Interval(0, false, 100, true).LinSpace();

            for (int i = 1; i <= 100; i++)
                Assert.AreEqual(i, points[i - 1]);
        }

        [TestMethod]
        public void to_open__excludes_to()
        {
            // [1;101)
            var points = new Interval(1, true, 101, false).LinSpace();

            for (int i = 1; i <= 100; i++)
                Assert.AreEqual(i, points[i - 1]);
        }
    }
}
