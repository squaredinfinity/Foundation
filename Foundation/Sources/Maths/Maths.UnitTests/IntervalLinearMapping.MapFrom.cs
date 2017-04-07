﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths.UnitTests
{
    [TestClass]
    public class IntervalLinearMapping__MapFrom
    {
        [TestMethod]
        public void Zero_based_target__Works()
        {
            var m = new IntervalLinearMapping(new Interval(-100, 100), new Interval(0, 1));

            Assert.AreEqual(-100, m.MapFrom(0));
            Assert.AreEqual(0, m.MapFrom(0.5));
            Assert.AreEqual(100, m.MapFrom(1));
        }

        [TestMethod]
        public void Inverted_zero_based_target____Works()
        {
            var m = new IntervalLinearMapping(new Interval(-100, 100), new Interval(1, 0));

            Assert.AreEqual(100, m.MapFrom(0));
            Assert.AreEqual(0, m.MapFrom(0.5));
            Assert.AreEqual(-100, m.MapFrom(1));
        }

        [TestMethod]
        public void Non_zero_based_target__Works()
        {
            var m = new IntervalLinearMapping(new Interval(-100, 100), new Interval(100, 200));

            Assert.AreEqual(-100, m.MapFrom(100));
            Assert.AreEqual(0, m.MapFrom(150));
            Assert.AreEqual(100, m.MapFrom(200));
        }

        [TestMethod]
        public void Inverted_non_zero_based_target____Works()
        {
            var m = new IntervalLinearMapping(new Interval(-100, 100), new Interval(200, 100));

            Assert.AreEqual(100, m.MapFrom(100));
            Assert.AreEqual(0, m.MapFrom(150));
            Assert.AreEqual(-100, m.MapFrom(200));
        }
    }
}
