using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SquaredInfinity.Foundation.Maths.Space2D
{
	[TestClass]
    public class RectangleTests
    {
        [TestMethod]
        public void Clips_TopLeft_Preserves_RightBottom()
        {
            var r1 = new Rectangle(-25, -50, 75, 100);

            var old_right = r1.Right;
            var old_bottom = r1.Bottom;

            r1.Clip(new Rectangle(0, 0, 200, 300));

            Assert.AreEqual(0, r1.X);
            Assert.AreEqual(0, r1.Y);
            Assert.AreEqual(old_right, r1.Right);
            Assert.AreEqual(old_bottom, r1.Bottom);
        }

        [TestMethod]
        public void Clip_area_contains_original__no_clipping()
        {
            var r1 = new Rectangle(25, 50, 75, 100);

            r1.Clip(new Rectangle(0, 0, 200, 200));

            Assert.AreEqual(25, r1.X);
            Assert.AreEqual(50, r1.Y);
            Assert.AreEqual(75, r1.Width);
            Assert.AreEqual(100, r1.Height);
        }
    }
}
