using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Tagging;

namespace Tagging.UnitTests
{
    [TestClass]
    public class TagCollection__Add
    {
        [TestMethod]
        public void no_value_specified__adds_marker_value()
        {
            var tc = new TagCollection();

            tc.Add("isSet");
            Assert.IsTrue((bool)tc["isSet"]);
            Assert.IsTrue(tc.GetAllRawValues()[0].Value is MarkerValue);

            tc.Add(new Tag("isSet2"));
            Assert.IsTrue((bool)tc["isSet2"]);
            Assert.IsTrue(tc.GetAllRawValues()[0].Value is MarkerValue);
        }

        [TestMethod]
        public void value_not_exist__adds_value()
        {
            var tc = new TagCollection();

            tc.Add("a", 1);
            Assert.AreEqual(1, tc["a"]);
            Assert.IsTrue(tc.GetAllRawValues()[0].Value is int);

            tc.Add(new Tag("b"), 2);
            Assert.AreEqual(2, tc["b"]);
            Assert.IsTrue(tc.GetAllRawValues()[0].Value is int);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void value_exists__throws()
        {
            var tc = new TagCollection();

            tc.Add("a", 1);
            Assert.AreEqual(1, tc["a"]);

            tc.Add(new Tag("a"), 2);
            Assert.Fail("it should have thrown");
        }

        [TestMethod]
        public void tag_type_single__adds_value()
        {
            var tc = new TagCollection();

            tc.Add("a", 1, TagType.SingleValue);
            Assert.IsFalse(tc.GetAllRawValues()[0].Value is MultiValue);
        }

        [TestMethod]
        public void tag_type_multi__adds_value()
        {
            var tc = new TagCollection();

            tc.Add("a", 1, TagType.MultiValue);
            Assert.AreEqual(1, tc["a"]);
            Assert.IsTrue(tc.GetAllRawValues()[0].Value is MultiValue);
        }
    }
}
