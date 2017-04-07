using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Graphics.Drawing;

namespace Graphics.Drawing.Tests
{
    [TestClass]
    public class PixelCanvas_DrawLineWu
    {
        [TestMethod]
        public void Diagonal_1()
        {
            for (var thickness = 1; thickness <= 5; thickness++)
            {
                var pc = new PixelArrayCanvas(100, 100);
                pc.DrawLineWu(0, 0, 99, 99, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                pc.Save($@"E:\t\line wu\diagonal_{thickness}.png");
            }
        }
    }
}
