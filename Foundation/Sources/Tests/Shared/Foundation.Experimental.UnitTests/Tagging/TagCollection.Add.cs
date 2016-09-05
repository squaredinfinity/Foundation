using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Tagging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.UnitTests.Tagging
{
    [TestClass]
    public class TagCollection_Add
    {
        [TestMethod]
        public void value_not_specified__value_set_to_true()
        {
            ITagCollection tc = new TagCollection();
            tc.Add("should_be_true");

            Assert.IsTrue((bool)tc["should_be_true"].Single());
            Assert.IsTrue(tc.Contains("should_be_true"));
            Assert.AreEqual(1, tc.Count());
        }
    }
}
