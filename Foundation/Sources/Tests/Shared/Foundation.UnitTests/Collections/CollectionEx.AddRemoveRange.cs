using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
	[TestClass]
    public class CollectionEx__AddRemoveRange
    {
        [TestMethod]
        public void CanAddRange()
        {
            var c = new CollectionEx<int>();

            c.AddRange(new int[] { 1, 2 });

            Assert.IsTrue(c[0] == 1);
            Assert.IsTrue(c[1] == 2);
            Assert.AreEqual(2, c.Count);
        }

        [TestMethod]
        public void CanRemoveRange()
        {
            var c = new CollectionEx<int>();

            c.AddRange(new int[] { 1, 2 });
            c.RemoveRange(0, 2);

            Assert.AreEqual(0, c.Count);
        }
    }
}
