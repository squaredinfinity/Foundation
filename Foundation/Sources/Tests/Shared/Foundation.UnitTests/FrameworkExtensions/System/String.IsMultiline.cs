using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class String__IsMultiline
    {
        [TestMethod]
        public void StringHasMultipleLines__ReturnsTrue()
        {
            var s = "1" + Environment.NewLine + "2";

            var r = s.IsMultiLine();

            Assert.IsTrue(r);
        }

        [TestMethod]
        public void StringDoesNotHaveMultipleLines__ReturnsFalse()
        {
            var s = "1" + "2";

            var r = s.IsMultiLine();

            Assert.IsFalse(r);
        }
    }
}
