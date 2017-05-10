using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths.UnitTests
{
    [TestClass]
    public class Rectangle__Union
    {
        [TestMethod]
        public void Empty_and_Empty__Empty()
        {
            var a = Rectangle.Empty;
            var b = Rectangle.Empty;

            a.Union(b);

            Assert.IsTrue(a.IsEmpty);
        }

        [TestMethod]
        public void Empty_and_NonEmpty__SameAsNonEmpty()
        {
            var a = Rectangle.Empty;
            var b = new Rectangle(10, 20, 30, 40);

            a.Union(b);

            Assert.AreEqual(10, a.X);
            Assert.AreEqual(20, a.Y);
            Assert.AreEqual(30, a.Width);
            Assert.AreEqual(40, a.Height);
        }
    }
}
