using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class String__Substring
    {
        [TestMethod]
        public void ValidRegexSpecified__ReturnsMatchedSubstring()
        {
            var ss = "test 123 test".Substring(@"t \d+ t");

            Assert.AreEqual("t 123 t", ss);
        }

        [TestMethod]
        public void ValidRegexAndCapturingGroupSpecified__ReturnsMatchedCapturingGroup()
        {
            var ss = "test 123 test".Substring(@"t (?<numbers>\d+) t", "numbers");

            Assert.AreEqual("123", ss);
        }
    }
}
