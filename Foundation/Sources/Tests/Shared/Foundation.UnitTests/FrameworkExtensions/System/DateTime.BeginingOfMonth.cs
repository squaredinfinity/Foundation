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
    public class DateTime__BeginingOfMonth
    {
        [TestMethod]
        public void ReturnsDateTimeSetToBeginingOfMonth()
        {
            var dt = new DateTime(2014, 01, 13).BeginingOfMonth();
            
            Assert.AreEqual(2014, dt.Year);
            Assert.AreEqual(01, dt.Month);
            Assert.AreEqual(01, dt.Day);
            Assert.AreEqual(TimeSpan.Zero, dt.TimeOfDay);

            dt = new DateTime(2014, 02, 13).BeginingOfMonth();

            Assert.AreEqual(2014, dt.Year);
            Assert.AreEqual(02, dt.Month);
            Assert.AreEqual(01, dt.Day);
            Assert.AreEqual(TimeSpan.Zero, dt.TimeOfDay);

            dt = new DateTime(2014, 12, 31).BeginingOfMonth();

            Assert.AreEqual(2014, dt.Year);
            Assert.AreEqual(12, dt.Month);
            Assert.AreEqual(01, dt.Day);
            Assert.AreEqual(TimeSpan.Zero, dt.TimeOfDay);
        }

        [TestMethod]
        public void BeginingOfThisMonth()
        {
            var dt = DateTimeExtensions.BeginingOfThisMonth();
            var dt2 = DateTime.Now.BeginingOfMonth();
            Assert.AreEqual(dt, dt2);

            dt = DateTimeExtensions.BeginingOfThisMonth();
            dt2 = DateTime.Now.BeginingOfMonth();
            Assert.AreEqual(dt, dt2);

            dt = DateTimeExtensions.BeginingOfThisMonth();
            dt2 = DateTime.Now.BeginingOfMonth();
            Assert.AreEqual(dt, dt2);

            dt = DateTimeExtensions.BeginingOfThisMonth();
            dt2 = DateTime.Now.BeginingOfMonth();
            Assert.AreEqual(dt, dt2);
        }
    }
}
