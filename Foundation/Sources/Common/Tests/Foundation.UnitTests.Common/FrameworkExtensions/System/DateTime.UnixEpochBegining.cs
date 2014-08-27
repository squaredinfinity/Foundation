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
    public class DateTime__UnixEpochBegining
    {
        [TestMethod]
        public void ReturnsValidDate()
        {
            var ueb = DateTimeExtensions.UnixEpochBegining();

            Assert.AreEqual(1970, ueb.Year);
            Assert.AreEqual(1, ueb.Month);
            Assert.AreEqual(1, ueb.Day);
        }
    }
}
