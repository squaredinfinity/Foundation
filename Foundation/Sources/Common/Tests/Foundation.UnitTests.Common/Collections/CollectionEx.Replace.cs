using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
	[TestClass]
    public class CollectionEx__Replace
    {
        [TestMethod]
        public void CanReplaceItems()
        {
            var c = new CollectionEx<int>();

            c.Add(1);
            c.Add(2);

            c.Replace(1, 3);

            Assert.IsTrue(c[0] == 3);
            Assert.IsTrue(c[1] == 2);
            Assert.AreEqual(2, c.Count);
        }
    }
}
