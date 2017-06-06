using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Collections;
using System.Collections.Generic;

namespace Collections.Core.UnitTests
{
    [TestClass]
    public class IIndexFilter__General
    {
        List<int> GetDefaultList() => new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        [TestMethod]
        public void by_default_adds_all_indexes()
        {
            var ixf = new IndexFilter<int>(GetDefaultList());

            Assert.AreEqual(10, ixf.Count);
            
            for(int i = 0; i < ixf.Count; i++)
            {
                Assert.AreEqual(i + 1, ixf[i]);
            }
        }

        [TestMethod]
        public void can_initialize_with_specific_indexes()
        {
            var ixf = new IndexFilter<int>(GetDefaultList(), 0, 9);

            Assert.AreEqual(2, ixf.Count);

            Assert.AreEqual(1, ixf[0]);
            Assert.AreEqual(10, ixf[1]);
        }

        [TestMethod]
        public void can_initialize_with_no_indexes()
        {
            var ixf = new IndexFilter<int>(GetDefaultList(), new int[0]);

            Assert.AreEqual(0, ixf.Count);
        }

        [TestMethod]
        public void can_add_indexes()
        {
            // start with no indexes
            var ixf = new IndexFilter<int>(GetDefaultList(), new int[0]);

            Assert.AreEqual(0, ixf.Count);

            ixf.AddIndex(0);
            Assert.AreEqual(1, ixf.Count);
            Assert.AreEqual(1, ixf[0]);

            ixf.AddIndex(9);
            Assert.AreEqual(2, ixf.Count);
            Assert.AreEqual(10, ixf[1]);
        }

        [TestMethod]
        public void can_remove_indexes()
        {
            // start with all indexes
            var ixf = new IndexFilter<int>(GetDefaultList());

            Assert.AreEqual(10, ixf.Count);

            ixf.TryRemoveIndex(0);
            Assert.AreEqual(9, ixf.Count);
            Assert.AreEqual(2, ixf[0]);
        }
    }
}
