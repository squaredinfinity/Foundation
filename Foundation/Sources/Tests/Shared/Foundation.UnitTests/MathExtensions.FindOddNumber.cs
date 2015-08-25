using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.UnitTests
{
    [TestClass]
    public partial class MathExtensionsTests
    {
        [TestMethod]
        public void FindOddNumber__DoesntTreatZeroAsOdd()
        {
            var odd = MathExtensions.FindOddNumber(0, ForwardBackwardDirection.Forward);

            Assert.AreEqual(1, odd);
        }

        [TestMethod]
        public void FindOddNumber__ReturnsOriginIfOdd()
        {
            var odd = MathExtensions.FindOddNumber(1, ForwardBackwardDirection.Forward);

            Assert.AreEqual(1, odd);
        }

        [TestMethod]
        public void FindOddNumber__CanDoForwardLookup()
        {
            var odd = MathExtensions.FindOddNumber(0, ForwardBackwardDirection.Forward);

            Assert.AreEqual(1, odd);
        }

        [TestMethod]
        public void FindOddNumber__CanDoBackwardLookup()
        {
            var odd = MathExtensions.FindOddNumber(0, ForwardBackwardDirection.Backward);

            Assert.AreEqual(-1, odd);
        }
    }
}
