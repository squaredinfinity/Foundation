
using System.Linq;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Tagging;

namespace Tagging.UnitTests
{
    [TestClass]
    public class TagCollection__AddOrAppend
    {
        [TestMethod]
        public void value_not_exist__adds_value_as_multi_value()
        {
            var tc = new TagCollection();

            tc.AddOrAppend("a", 1);
            Assert.AreEqual(1, tc["a"]);
            Assert.IsTrue(tc.GetAllRawValues()[0].Value is MultiValue);

            tc.AddOrAppend(new Tag("b"), 2);
            Assert.AreEqual(2, tc["b"]);
            Assert.IsTrue(tc.GetAllRawValues()[0].Value is MultiValue);
        }

        [TestMethod]
        public void value_exists_as_multi__appends_value()
        {
            var tc = new TagCollection();

            tc.Add("a", 1, TagType.MultiValue);

            tc.AddOrAppend("a", 2);
            Assert.IsTrue(tc.GetAllRawValues()[0].Value is MultiValue);

            tc.AddOrAppend(new Tag("a"), 3);
            var vals = tc.GetAllValues("a");
            Assert.AreEqual(3, vals.Count);
            Assert.IsTrue(vals.Contains(1));
            Assert.IsTrue(vals.Contains(2));
            Assert.IsTrue(vals.Contains(3));

            Assert.IsTrue(tc.GetAllRawValues()[0].Value is MultiValue);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void value_exists_as_single__throws()
        {
            var tc = new TagCollection();

            tc.Add("a", 1, TagType.SingleValue);

            tc.AddOrAppend("a", 2);

            Assert.Fail("should have thrown");
        }
    }
}
