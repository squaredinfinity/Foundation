using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Maths;
using SquaredInfinity.Foundation.Extensions;
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
            var even = -1.FindEvenNumber(ForwardBackwardDirection.Forward);

            Assert.AreEqual(0, even);
        }

        [TestMethod]
        public void FindEvenNumber__ReturnsOriginIfEven()
        {
            var even = 0.FindEvenNumber(ForwardBackwardDirection.Forward);

            Assert.AreEqual(0, even);
        }

        [TestMethod]
        public void FindEvenNumber__CanDoForwardLookup()
        {
            var even = 1.FindEvenNumber(ForwardBackwardDirection.Forward);

            Assert.AreEqual(2, even);
        }

        [TestMethod]
        public void FindEvenNumber__CanDoBackwardLookup()
        {
            var even = (-1).FindEvenNumber(ForwardBackwardDirection.Backward);

            Assert.AreEqual(-2, even);
        }
    }
}
