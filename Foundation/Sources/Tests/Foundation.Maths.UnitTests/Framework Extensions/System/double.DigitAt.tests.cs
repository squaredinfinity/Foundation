using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Framework_Extensions.System
{
    [TestClass]
    public class Double__DitgitAt
    {
        [TestMethod]
        public void ReturnsSpecifiedDigit()
        {
            var x = 1.255;

            // requested order of magnitude greater than number order of magnitude
            Assert.AreEqual(0, x.DigitAt(3));
            Assert.AreEqual(0, x.DigitAt(2));
            Assert.AreEqual(0, x.DigitAt(1));


            Assert.AreEqual(1, x.DigitAt(0));
            Assert.AreEqual(2, x.DigitAt(-1));
            Assert.AreEqual(5, x.DigitAt(-2));
            Assert.AreEqual(5, x.DigitAt(-3));
        }
    }
}
