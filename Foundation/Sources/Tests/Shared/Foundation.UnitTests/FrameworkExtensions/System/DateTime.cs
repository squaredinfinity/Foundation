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
    public class DateTimeTests
    {
        [TestMethod]
        public void IsToday()
        {
            var dt = DateTime.Now;
            Assert.IsTrue(dt.IsToday());
        }

        [TestMethod]
        public void IsCurrentWeek()
        {
            var dt = DateTime.Now;
            Assert.IsTrue(dt.IsCurrentWeek());
        }

        [TestMethod]
        public void IsCurrentMonth()
        {
            var dt = DateTime.Now;
            Assert.IsTrue(dt.IsCurrentMonth());

            dt = DateTime.Now.BeginingOfMonth().Subtract(TimeSpan.FromTicks(1));
            Assert.IsFalse(dt.IsCurrentMonth());
        }
    }
}
