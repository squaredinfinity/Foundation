using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    [TestClass]
    public class IsNullOrEmptyTests
    {
        [TestMethod]
        public void EmptyString__ReturnsTrue()
        {
            var s = "";

            var r = s.IsNullOrEmpty();

            Assert.IsTrue(r);
        }

        [TestMethod]
        public void NonEmptyString__ReturnsFalse()
        {
            var s = "1";

            var r = s.IsNullOrEmpty();

            Assert.IsFalse(r);
        }

        [TestMethod]
        public void StringContainsWhitespaceCharactersOnly_TreatWhitespaceOnlyAsEmptyStringIsTrue__ReturnsTrue()
        {
            var s = "\t ";

            var r = s.IsNullOrEmpty(treatWhitespaceOnlyStringAsEmpty: true);

            Assert.IsTrue(r);
        }

        [TestMethod]
        public void StringContainsWhitespaceCharactersOnly_TreatWhitespaceOnlyAsEmptyStringIsFalse__ReturnsFalse()
        {
            var s = "\t ";

            var r = s.IsNullOrEmpty(treatWhitespaceOnlyStringAsEmpty: false);

            Assert.IsFalse(r);
        }
    }
}
