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
    public class Rectangle__Empty
    {
        [TestMethod]
        public void coordiantes_and_dimensions_are_NaN()
        {
            var r = Rectangle.Empty;

            Assert.IsTrue(double.IsNaN(r.X));
            Assert.IsTrue(double.IsNaN(r.Y));
            Assert.IsTrue(double.IsNaN(r.Width));
            Assert.IsTrue(double.IsNaN(r.Height));
        }

        [TestMethod]
        public void IsEmpty()
        {
            var r = Rectangle.Empty;

            Assert.IsTrue(r.IsEmpty);
        }

        [TestMethod]
        public void NotAtOrigin()
        {
            var r = Rectangle.Empty;

            Assert.IsFalse(r.IsAtOrigin);
        }
    }
}
