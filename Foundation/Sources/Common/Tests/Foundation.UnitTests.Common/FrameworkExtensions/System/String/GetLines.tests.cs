using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    [TestClass]
    public class GetLinesTests
    {
        [TestMethod]
        public void SingleLineString__ReturnsStringItself()
        {
            var s = "123";

            var r = s.GetLines();

            Assert.IsTrue(r.Length == 1);
            Assert.AreEqual(s, r[0]);
        }

        [TestMethod]
        public void MultiLineString__ReturnsAllLinesWithoutLineBreaks()
        {
            var s = "1" + Environment.NewLine + "2" + Environment.NewLine + "3";

            var r = s.GetLines();

            Assert.IsTrue(r.Length == 3);
            Assert.AreEqual("1", r[0]);
            Assert.AreEqual("2", r[1]);
            Assert.AreEqual("3", r[2]);
        }

        [TestMethod]
        public void MultiLineString_KeppLineBreaksIsTrue__ReturnsAllLinesWithLineBreaks()
        {
            var s = "1" + Environment.NewLine + "2" + Environment.NewLine + "3";

            var r = s.GetLines(keepLineBreaks: true);

            Assert.IsTrue(r.Length == 3);
            Assert.AreEqual("1" + Environment.NewLine, r[0]);
            Assert.AreEqual("2" + Environment.NewLine, r[1]);
            Assert.AreEqual("3", r[2]);
        }

        [TestMethod]
        public void MultilneString_EndsWithEmptyLine__LastReturnedLineIsEmpty()
        {
            var s = "1" + Environment.NewLine;

            var r = s.GetLines();

            Assert.IsTrue(r.Length == 2);
            Assert.AreEqual("1", r[0]);
            Assert.AreEqual("", r[1]);
        }
    }
}
