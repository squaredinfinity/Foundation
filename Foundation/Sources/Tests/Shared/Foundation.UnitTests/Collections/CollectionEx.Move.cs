using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
	[TestClass]
    public class CollectionEx__Move
    {
        [TestMethod]
        public void CanMoveItems()
        {
            var c = new CollectionEx<int>();

            c.Add(1);
            c.Add(2);
            
            // swap 1 and 2
            c.Move(0, 1);

            Assert.IsTrue(c[0] == 2);
            Assert.IsTrue(c[1] == 1);
            Assert.AreEqual(2, c.Count);

            // swap back again
            c.Move(1, 0);
            Assert.IsTrue(c[0] == 1);
            Assert.IsTrue(c[1] == 2);
            Assert.AreEqual(2, c.Count);
        }
    }
}
