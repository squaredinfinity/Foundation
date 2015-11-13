using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Media.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Unsafe.UnitTests.Media.Drawing
{
    [TestClass]
    public class UnsafePixelCanvasTests
    {
        [TestMethod]
        public void MyTestMethod()
        {
            var pc = new UnsafePixelCanvas(50, 50);
            pc.DrawLineDDA(0, 0, 50, 50, pc.GetColor(255,0,0,0));

            var x = pc.GetPixels();
        }
    }
}
