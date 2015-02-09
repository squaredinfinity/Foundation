using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
	[TestClass]
    public class CollectionExTests
    {
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void IsReadonly_Add_ThrowsException()
        {
            var c = new CollectionEx<int>(new int[] { 1, 2 });

            c.Add(3);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void IsReadonly_RemoveAt_ThrowsException()
        {
            var c = new CollectionEx<int>(new int[] { 1, 2 });

            c.RemoveAt(0);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void IsReadonly_Remove_ThrowsException()
        {
            var c = new CollectionEx<int>(new int[] { 1, 2 });

            c.Remove(1);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void IsReadonly_Clear_ThrowsException()
        {
            var c = new CollectionEx<int>(new int[] { 1, 2 });

            c.Clear();
        }
    }
}
