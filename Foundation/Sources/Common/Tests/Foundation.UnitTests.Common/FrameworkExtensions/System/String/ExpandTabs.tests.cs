using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class ExpandTabsTests
    {
        [TestMethod]
        public void EmptyString__ReturnsEmptyString()
        {
            var s = "";

            var r = s.ExpandTabs();

            Assert.AreEqual(s, r);
        }

        [TestMethod]
        public void NoTabs__ReturnsActualString()
        {
            var s = "a";
            var r = s.ExpandTabs();
            Assert.AreEqual(s, r);

            s = "aaaaa";
            r = s.ExpandTabs();
            Assert.AreEqual(s, r);
        }

        [TestMethod]
        public void StringWithTabs__ExpandsTabs()
        {
            var s = "\t";
            var r = s.ExpandTabs();
            Assert.AreEqual("    ", r);

            s = "\t\t";
            r = s.ExpandTabs();
            Assert.AreEqual("        ", r);

            s = "a\t";
            r = s.ExpandTabs();
            Assert.AreEqual("a   ", r);

            s = "aa\t";
            r = s.ExpandTabs();
            Assert.AreEqual("aa  ", r);

            s = "aaa\t";
            r = s.ExpandTabs();
            Assert.AreEqual("aaa ", r);

            s = "aaaa\t";
            r = s.ExpandTabs();
            Assert.AreEqual("aaaa    ", r);

            s = "aaaaa\t";
            r = s.ExpandTabs();
            Assert.AreEqual("aaaaa   ", r);

            s = "aaaaaa\t";
            r = s.ExpandTabs();
            Assert.AreEqual("aaaaaa  ", r);

            s = "aaaaaaa\t";
            r = s.ExpandTabs();
            Assert.AreEqual("aaaaaaa ", r);

            s = "aaaaaaaa\t";
            r = s.ExpandTabs();
            Assert.AreEqual("aaaaaaaa    ", r);

            s = "\ta";
            r = s.ExpandTabs();
            Assert.AreEqual("    a", r);

            s = "\ta\t";
            r = s.ExpandTabs();
            Assert.AreEqual("    a   ", r);
        }

        [TestMethod]
        public void MultiLineStringWithTabs__ExpandsTabs()
        {
            var s = "\t" + Environment.NewLine + "\t";
            var r = s.ExpandTabs();

            var expected = "    " + Environment.NewLine + "    ";

            Assert.AreEqual(expected, r);
        }
    }
}
