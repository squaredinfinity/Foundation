using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.FrameworkExtensions.System.Collections
{
    [TestClass]
    public class IEnumerableExtensions__ToList
    {
        [TestMethod]
        public void ToList__SetsSpecifiedCapacity()
        {
            var source = new int[] { 1, 2 };

            var list = source.ToList(capacity: 11);

            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(11, list.Capacity);
        }
    }
}
