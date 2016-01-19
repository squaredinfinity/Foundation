using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class Double__AreClose
    {
        [TestMethod]
        public void DefaultPrecision__ShouldChangeDoubleNumber()
        {
            // default precision should be a number that 1.0 + DefaultPrecision != 1.0

            var d = 1.0 + DoubleExtensions.DefaultPrecision;

            Assert.IsTrue(d != 1.0);

            // 1.0 + (defaultprecision / 2.0) == 1.0 because it's too small to change a double number

            d = 1.0 + (DoubleExtensions.DefaultPrecision / 2.0);

            Assert.IsTrue(d == 1.0);
        }

        [TestMethod]
        public void NumbersAreWithinPrecision__ReturnsTrue()
        {
            var d = 0.1 * 0.1;

            var r = DoubleExtensions.AreClose(d, 0.01);

            Assert.IsTrue(r);
        }

        [TestMethod]
        public void Infinity_IsNotCloseTo_InfinityWithSameSign()
        {
            var d = double.NegativeInfinity;
            var d2 = double.NegativeInfinity;
            var r = DoubleExtensions.AreClose(d, d2);

            Assert.IsFalse(r);


            d = double.PositiveInfinity;
            d2 = double.PositiveInfinity;
            r = DoubleExtensions.AreClose(d, d2);

            Assert.IsFalse(r);
        }

        [TestMethod]
        public void Infinity_IsNotCloseTo_InfinityWithDifferentSign()
        {
            var d = double.NegativeInfinity;
            var d2 = double.PositiveInfinity;
            var r = DoubleExtensions.AreClose(d, d2);

            Assert.IsFalse(r);


            d = double.PositiveInfinity;
            d2 = double.NegativeInfinity;
            r = DoubleExtensions.AreClose(d, d2);

            Assert.IsFalse(r);
        }

        [TestMethod]
        public void NaN_IsNotCloseTo_NaN()
        {
            var d = double.NaN;
            var d2 = double.NaN;
            var r = DoubleExtensions.AreClose(d, d2);

            Assert.IsFalse(r);
        }
    }
}
