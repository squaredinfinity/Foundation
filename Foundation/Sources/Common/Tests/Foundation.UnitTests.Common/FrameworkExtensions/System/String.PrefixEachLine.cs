using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class String__PrefixEachLine
    {
        [TestMethod]
        public void SingleLineString__AddsPrefix()
        {
            var s = "1";

            var r = s.PrefixEachLine("-");

            Assert.AreEqual("-1", r);
        }

        [TestMethod]
        public void MultilineString__AddsPrefixToEachLine()
        {
            var s = "1" + Environment.NewLine + "2" + Environment.NewLine + "3";

            var r = s.PrefixEachLine("-");

            var expected = "-1" + Environment.NewLine + "-2" + Environment.NewLine + "-3";

            Assert.AreEqual(expected, r);
        }
    }
}
