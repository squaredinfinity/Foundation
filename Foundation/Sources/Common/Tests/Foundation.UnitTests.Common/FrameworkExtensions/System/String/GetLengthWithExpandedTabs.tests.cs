using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class GetLengthWithExpandedTabsTests
    {
        [TestMethod]
        public void EmptyString__Zero()
        {
            var s = "";

            var r = s.GetLengthWithExpandedTabs();

            Assert.AreEqual(0, r);
        }

        [TestMethod]
        public void NoTabs__ReturnsActualStringLength()
        {
            var s = "a";
            var r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(1, r);

            s = "aaaaa";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(5, r);
        }

        [TestMethod]
        public void StringWithTabs__GetsTheLength()
        {
            var s = "\t";
            var r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(4, r);

            s = "\t\t";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(8, r);

            s = "a\t";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(4, r);

            s = "aa\t";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(4, r);

            s = "aaa\t";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(4, r);

            s = "aaaa\t";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(8, r);

            s = "aaaaa\t";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(8, r);

            s = "aaaaaa\t";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(8, r);

            s = "aaaaaaa\t";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(8, r);

            s = "aaaaaaaa\t";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(12, r);

            s = "\ta";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(5, r);

            s = "\ta\t";
            r = s.GetLengthWithExpandedTabs();
            Assert.AreEqual(8, r);
        }
    }
}
