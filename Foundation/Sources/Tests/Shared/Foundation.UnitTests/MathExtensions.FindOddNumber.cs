using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Maths;
using SquaredInfinity.Foundation.Extensions;
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
            var odd = 0.FindOddNumber(ForwardBackwardDirection.Forward);

            Assert.AreEqual(1, odd);
        }

        [TestMethod]
        public void FindOddNumber__ReturnsOriginIfOdd()
        {
            var odd = 1.FindOddNumber(ForwardBackwardDirection.Forward);

            Assert.AreEqual(1, odd);
        }

        [TestMethod]
        public void FindOddNumber__CanDoForwardLookup()
        {
            var odd = 0.FindOddNumber(ForwardBackwardDirection.Forward);

            Assert.AreEqual(1, odd);
        }

        [TestMethod]
        public void FindOddNumber__CanDoBackwardLookup()
        {
            var odd = 0.FindOddNumber(ForwardBackwardDirection.Backward);

            Assert.AreEqual(-1, odd);
        }
    }
}
