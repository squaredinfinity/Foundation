using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
	[TestClass]
    public class CollectionEx__Clear
    {
        [TestMethod]
        public void CanClearItems()
        {
            var c = new CollectionEx<int>();

            c.Add(1);
            c.Add(2);

            c.Clear();

            Assert.AreEqual(0, c.Count);
        }
    }
}
