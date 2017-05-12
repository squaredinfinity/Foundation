using System;
using System.Linq; 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Tagging;

namespace Tagging.UnitTests
{
    [TestClass]
    public class TagCollection__AddUpdateOrAppendFrom
    {
        [TestMethod]
        public void value_not_exist__adds_value()
        {
            var tc1 = new TagCollection();
            tc1.Add("a", 1);

            var tc2 = new TagCollection();
            tc2.AddOrUpdateFrom(tc1);
            Assert.AreEqual(1, tc2["a"]);
        }

        [TestMethod]
        public void value_exists_as_single_target_multi__replaces_single_with_multi()
        {
            var tc1 = new TagCollection();
            tc1.Add("a", 1, TagType.MultiValue);
            tc1.AddOrAppend("a", 2);

            var tc2 = new TagCollection();
            tc2.Add("a", 3, TagType.SingleValue);
            tc2.AddOrUpdateFrom(tc1);
            var vals = tc2.GetAll("a");
            Assert.AreEqual(2, vals.Count);
            Assert.IsTrue(vals.Contains(1));
            Assert.IsTrue(vals.Contains(2));
            Assert.AreEqual(1, tc2.GetAllRawValues().Count);
        }
    }
}
