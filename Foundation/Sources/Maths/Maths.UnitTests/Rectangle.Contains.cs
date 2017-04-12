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
    public class Rectangle__Contains
    {
        [TestMethod]
        public void Point_outside__returns_false()
        {
            var r = new Rectangle(0, 0, 10, 10);
            var p = new Point2D(50, 50);

            Assert.IsFalse(r.Contains(p));
        }

        [TestMethod]
        public void point_at_edges__returns_true()
        {
            var r = new Rectangle(0, 0, 10, 10);

            var p = new Point2D(0, 0);
            Assert.IsTrue(r.Contains(p));

            p = new Point2D(0, 5);
            Assert.IsTrue(r.Contains(p));

            p = new Point2D(0, 10);
            Assert.IsTrue(r.Contains(p));

            p = new Point2D(5, 10);
            Assert.IsTrue(r.Contains(p));

            p = new Point2D(10, 0);
            Assert.IsTrue(r.Contains(p));

            p = new Point2D(10, 5);
            Assert.IsTrue(r.Contains(p));

            p = new Point2D(10, 10);
            Assert.IsTrue(r.Contains(p));
        }

        [TestMethod]
        public void points_at_edges__returns_true()
        {
            var points = new[]
            {
                new Point2D(0, 0),
                new Point2D(0, 5),
                new Point2D(0, 10),
                new Point2D(5, 10),
                new Point2D(10, 0),
                new Point2D(10, 5),
                new Point2D(10, 10)
            };

            var r = new Rectangle(0, 0, 10, 10);

            foreach(var p in points)
            {
                Assert.IsTrue(r.Contains(p));
            }

            r.Offset(50, 50);

            foreach (var p in points)
            {
                p.Offset(50, 50);

                Assert.IsTrue(r.Contains(p));
            }
        }
    }
}
