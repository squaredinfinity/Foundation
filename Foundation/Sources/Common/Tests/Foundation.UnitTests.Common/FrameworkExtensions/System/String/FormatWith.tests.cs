using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class FormatWithTests
    {
        [TestMethod]
        public void ReturnsFormattedString()
        {
            var s = "{0} + {1} = {2}".FormatWith(1, 2, 3);

            Assert.AreEqual("1 + 2 = 3", s);
        }
    }
}
