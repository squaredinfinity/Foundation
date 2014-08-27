using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.FrameworkExtensions.System
{
    [TestClass]
    public class DateTime__BeginingOfTheWeek
    {
        [TestMethod]
        public void ReturnsDateTimeSetToBeginingOfTheWeek()
        {
            var dt = new DateTime(2014, 01, 01).BeginingOfWeek(DayOfWeek.Monday);
            
            Assert.AreEqual(2013, dt.Year);
            Assert.AreEqual(12, dt.Month);
            Assert.AreEqual(30, dt.Day);
            Assert.AreEqual(TimeSpan.Zero, dt.TimeOfDay);

            dt = new DateTime(2014, 01, 01).BeginingOfWeek(DayOfWeek.Wednesday);

            Assert.AreEqual(2014, dt.Year);
            Assert.AreEqual(01, dt.Month);
            Assert.AreEqual(01, dt.Day);
            Assert.AreEqual(TimeSpan.Zero, dt.TimeOfDay);

            dt = new DateTime(2014, 01, 01).BeginingOfWeek(DayOfWeek.Sunday);

            Assert.AreEqual(2013, dt.Year);
            Assert.AreEqual(12, dt.Month);
            Assert.AreEqual(29, dt.Day);
            Assert.AreEqual(TimeSpan.Zero, dt.TimeOfDay);
        }

        [TestMethod]
        public void BeginingOfThisWeek()
        {
            var dt = DateTimeExtensions.BeginingOfThisWeek();
            var dt2 = DateTime.Now.BeginingOfWeek();
            Assert.AreEqual(dt, dt2);

            dt = DateTimeExtensions.BeginingOfThisWeek(DayOfWeek.Monday);
            dt2 = DateTime.Now.BeginingOfWeek(DayOfWeek.Monday);
            Assert.AreEqual(dt, dt2);

            dt = DateTimeExtensions.BeginingOfThisWeek(DayOfWeek.Wednesday);
            dt2 = DateTime.Now.BeginingOfWeek(DayOfWeek.Wednesday);
            Assert.AreEqual(dt, dt2);

            dt = DateTimeExtensions.BeginingOfThisWeek(DayOfWeek.Sunday);
            dt2 = DateTime.Now.BeginingOfWeek(DayOfWeek.Sunday);
            Assert.AreEqual(dt, dt2);
        }
    }
}
