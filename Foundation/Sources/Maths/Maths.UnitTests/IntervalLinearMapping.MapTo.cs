using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths.UnitTests
{
    [TestClass]
    public class IntervalLinearMapping__MapTo
    {
        [TestMethod]
        public void Zero_based_target__Works()
        {
            var m = new IntervalLinearMapping(new Interval(-100, 100), new Interval(0, 1));

            Assert.AreEqual(0, m.MapTo(-100));
            Assert.AreEqual(0.5, m.MapTo(0));
            Assert.AreEqual(1, m.MapTo(100));
        }

        [TestMethod]
        public void Inverted_zero_based_target__Works()
        {
            var m = new IntervalLinearMapping(new Interval(-100, 100), new Interval(1, 0));

            Assert.AreEqual(1, m.MapTo(-100));
            Assert.AreEqual(0.5, m.MapTo(0));
            Assert.AreEqual(0, m.MapTo(100));
        }

        [TestMethod]
        public void Non_zero_based_target__Works()
        {
            var m = new IntervalLinearMapping(new Interval(-100, 100), new Interval(100, 200));

            Assert.AreEqual(100, m.MapTo(-100));
            Assert.AreEqual(150, m.MapTo(0));
            Assert.AreEqual(200, m.MapTo(100));
        }

        [TestMethod]
        public void Inverted_non_zero_based_target__Works()
        {
            var m = new IntervalLinearMapping(new Interval(-100, 100), new Interval(200, 100));

            Assert.AreEqual(200, m.MapTo(-100));
            Assert.AreEqual(150, m.MapTo(0));
            Assert.AreEqual(100, m.MapTo(100));
        }
    }
}
