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
    public class IEnumerableExtensions__ToNullIfEmpty
    {
        [TestMethod]
        public void ToNullIfEmpty__ReturnsNullIfSourceIsNull()
        {
            var source = (List<int>)null;

            var result = source.ToNullIfEmpty();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ToNullIfEmpty__ReturnsNullIfSourceIsEmpty()
        {
            var source = new List<int>();

            var result = source.ToNullIfEmpty();

            Assert.IsNull(result);
        }
    }
}
