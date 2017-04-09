using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Graphics.Drawing;

namespace Graphics.Drawing.Tests
{
    [TestClass]
    public class PixelCanvas_DrawLineWu
    {
        [TestMethod]
        public void Diagonal()
        {
            for (var thickness = 1; thickness <= 5; thickness++)
            {
                var pc = new PixelArrayCanvas(100, 100);
                pc.DrawLineWu(0, 0, 99, 99, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                pc.Save($@"E:\t\line wu\diagonal_NW_SE_{thickness}.png");

                pc = new PixelArrayCanvas(100, 100);
                pc.DrawLineWu(99, 99, 0, 0, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                pc.Save($@"E:\t\line wu\diagonal_SE_NW_{thickness}.png");

                pc = new PixelArrayCanvas(100, 100);
                pc.DrawLineWu(0, 99, 99, 0, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                pc.Save($@"E:\t\line wu\diagonal_SW_NE_{thickness}.png");

                pc = new PixelArrayCanvas(100, 100);
                pc.DrawLineWu(99, 0, 0, 99, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                pc.Save($@"E:\t\line wu\diagonal_NE_SW_{thickness}.png");
            }
        }

        [TestMethod]
        public void Horisontal()
        {
            for (var thickness = 1; thickness <= 5; thickness++)
            {
                var pc = new PixelArrayCanvas(100, 100);
                pc.DrawLineWu(0, 50, 99, 50, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                pc.Save($@"E:\t\line wu\horisontal_W_E_{thickness}.png");

                pc = new PixelArrayCanvas(100, 100);
                pc.DrawLineWu(99, 50, 0, 50, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                pc.Save($@"E:\t\line wu\horisontal_E_W_{thickness}.png");
            }
        }

        [TestMethod]
        public void Vertical()
        {
            for (var thickness = 1; thickness <= 5; thickness++)
            {
                var pc = new PixelArrayCanvas(100, 100);
                pc.DrawLineWu(50, 0, 50, 99, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                pc.Save($@"E:\t\line wu\vertical_N_S_{thickness}.png");

                pc = new PixelArrayCanvas(100, 100);
                pc.DrawLineWu(50, 99, 50, 0, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                pc.Save($@"E:\t\line wu\vertical_S_N_{thickness}.png");
            }
        }


        [TestMethod]
        public void XMajor()
        {
            for (var thickness = 1; thickness <= 5; thickness++)
            {
                var pc = new PixelArrayCanvas(100, 100);
                pc.DrawLineWu(10, 0, 90, 99, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                pc.Save($@"E:\t\line wu\xmajor_{thickness}.png");

                pc = new PixelArrayCanvas(100, 100);
                pc.DrawLineWu(50, 99, 50, 0, PixelCanvas.GetColor(255, 255, 0, 0), thickness);
                pc.Save($@"E:\t\line wu\vertical_S_N_{thickness}.png");
            }
        }
    }
}
