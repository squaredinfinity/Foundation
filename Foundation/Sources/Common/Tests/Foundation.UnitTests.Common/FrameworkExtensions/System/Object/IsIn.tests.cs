using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class Object__IsIn
    {
        [TestMethod]
        public void ItemIsInSpecifiedList__ReturnsTrue()
        {
            var x = 1;

            var r = x.IsIn(3, 2, 1, 0);

            Assert.IsTrue(r);
        }

        [TestMethod]
        public void ItemIsNotInSpecifiedList__ReturnsFalse()
        {
            var x = 13;

            var r = x.IsIn(3, 2, 1, 0);

            Assert.IsFalse(r);
        }

        [TestMethod]
        public void UsesSpecifiedEqualityComparer()
        {
            var x = "b";

            var r = x.IsIn("A", "B", "C");

            Assert.IsFalse(r);

            r = x.IsIn(StringComparer.InvariantCultureIgnoreCase, "A", "B", "C");

            Assert.IsTrue(r);
        }
    }
}
