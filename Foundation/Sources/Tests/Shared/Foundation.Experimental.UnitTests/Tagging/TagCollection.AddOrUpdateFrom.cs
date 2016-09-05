using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Tagging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.UnitTests.Tagging
{
    [TestClass]
    public class TagCollection_AddOrUpdateFrom
    {
        [TestMethod]
        public void All_items_added__By_default_existing_items_remain_unchanged()
        {
            ITagCollection tc = new TagCollection();
            tc.Add("one", 1);
            tc.Add("one", 2);
            tc.Add("one", 3);

            tc.Add("two", 11);
            tc.Add("two", 12);

            ITagCollection tc2 = new TagCollection();
            tc2.Add("one", 0);
            tc2.Add("one", 1);

            tc2.AddOrUpdateFrom(tc);

            var vals = tc2["one"];

            Assert.AreEqual(4, vals.Count);
            Assert.IsTrue(vals.Contains(0));
            Assert.IsTrue(vals.Contains(1));
            Assert.IsTrue(vals.Contains(2));
            Assert.IsTrue(vals.Contains(3));


            vals = tc2["two"];

            Assert.AreEqual(2, vals.Count);
            Assert.IsTrue(vals.Contains(11));
            Assert.IsTrue(vals.Contains(12));

            Assert.AreEqual(6, tc2.Count());
        }

        [TestMethod]
        public void All_items_added__existing_items_removed()
        {
            ITagCollection tc = new TagCollection();
            tc.Add("one", 1);
            tc.Add("one", 2);
            tc.Add("one", 3);

            tc.Add("two", 11);
            tc.Add("two", 12);

            ITagCollection tc2 = new TagCollection();
            tc2.Add("one", 0);
            tc2.Add("one", 1);

            tc2.AddOrUpdateFrom(tc, overrideOldValues: true);

            var vals = tc2["one"];

            Assert.AreEqual(3, vals.Count);
            Assert.IsTrue(vals.Contains(1));
            Assert.IsTrue(vals.Contains(2));
            Assert.IsTrue(vals.Contains(3));

            vals = tc2["two"];

            Assert.AreEqual(2, vals.Count);
            Assert.IsTrue(vals.Contains(11));
            Assert.IsTrue(vals.Contains(12));

            Assert.AreEqual(5, tc2.Count());
        }
    }
}
