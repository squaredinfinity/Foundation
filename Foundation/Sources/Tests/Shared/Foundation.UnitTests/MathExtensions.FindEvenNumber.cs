using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.UnitTests
{
    public partial class MathExtensionsTests
    {
        [TestMethod]
        public void FindEvenNumber__TreatsZeroAsEven()
        {
            var even = MathExtensions.FindEvenNumber(-1, ForwardBackwardDirection.Forward);

            Assert.AreEqual(0, even);
        }

        [TestMethod]
        public void FindEvenNumber__ReturnsOriginIfEven()
        {
            var even = MathExtensions.FindEvenNumber(0, ForwardBackwardDirection.Forward);

            Assert.AreEqual(0, even);
        }

        [TestMethod]
        public void FindEvenNumber__CanDoForwardLookup()
        {
            var even = MathExtensions.FindEvenNumber(1, ForwardBackwardDirection.Forward);

            Assert.AreEqual(2, even);
        }

        [TestMethod]
        public void FindEvenNumber__CanDoBackwardLookup()
        {
            var even = MathExtensions.FindEvenNumber(-1, ForwardBackwardDirection.Backward);

            Assert.AreEqual(-2, even);
        }
    }
}
