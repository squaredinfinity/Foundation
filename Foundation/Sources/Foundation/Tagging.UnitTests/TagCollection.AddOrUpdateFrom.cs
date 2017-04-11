using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Tagging;

namespace Tagging.UnitTests
{
    [TestClass]
    public class TagCollection__AddOrUpdateFrom
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
        public void value_exists_as_single__updates_value()
        {
            var tc1 = new TagCollection();
            tc1.Add("a", 1);

            var tc2 = new TagCollection();
            tc2.Add("a", 2, TagType.SingleValue);
            tc2.AddOrUpdateFrom(tc1);
            Assert.AreEqual(1, tc2["a"]);
            Assert.AreEqual(1, tc2.GetAllRawValues().Count);
        }

        [TestMethod]
        public void value_exists_as_multi__updates_value_replacing_existing_values()
        {
            var tc1 = new TagCollection();
            tc1.Add("a", 1);

            var tc2 = new TagCollection();
            tc2.Add("a", 2, TagType.MultiValue);
            tc2.AddOrUpdateFrom(tc1);
            Assert.AreEqual(1, tc2["a"]);
            Assert.AreEqual(1, tc2.GetAllRawValues().Count);
            
            // keep it as multi
            Assert.IsTrue(tc2.GetAllRawValues()[0].Value is MultiValue);
        }
    }
}
