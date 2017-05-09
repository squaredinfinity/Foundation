using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths.UnitTests
{
    [TestClass]
    public class ArithmeticSequence__At
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void negative_index__throws_argument_exception()
        {
            var s = new ArithmeticSequence();
            s.At(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void zero_index__throws_argument_exception()
        {
            var s = new ArithmeticSequence();
            s.At(0);
        }

        [TestMethod]
        public void MyTestMethod()
        {
            var s = new ArithmeticSequence();
            Assert.AreEqual(1, s.At(1));
            Assert.AreEqual(999, s.At(999));
        }
    }
}
