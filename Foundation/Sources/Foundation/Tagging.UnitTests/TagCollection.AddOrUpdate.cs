using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Tagging;

namespace Tagging.UnitTests
{
    [TestClass]
    public class TagCollection__AddOrUpdate
    {
        [TestMethod]
        public void value_not_exist__adds_value()
        {
            var tc = new TagCollection();

            tc.AddOrUpdate("a", 1);
            Assert.AreEqual(1, tc["a"]);
            Assert.IsTrue(tc.GetAllRawValues()[0].Value is MultiValue);

            tc.AddOrUpdate(new Tag("b"), 2);
            Assert.AreEqual(2, tc["b"]);
            Assert.IsTrue(tc.GetAllRawValues()[0].Value is MultiValue);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void value_exists_as_single__throws()
        {
            var tc = new TagCollection();

            tc.Add("a", 1);

            tc.AddOrAppend("a", 2);

            Assert.Fail("should have thrown");
        }
    }
}
