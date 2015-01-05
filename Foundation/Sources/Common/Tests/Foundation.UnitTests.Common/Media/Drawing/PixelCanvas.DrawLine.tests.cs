using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    [TestClass]
    public class PixelCanvas__DrawLine
    {
        [TestMethod]
        public void DDA()
        {
            var pc = new PixelCanvas(10, 10) as IGdiPixelCanvas;
            pc.DrawLineDDA(0, 0, 10, 10, pc.GetColor(System.Drawing.Color.DeepPink));

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(10, 10);

            for (int w = 0; w < pc.Width; w++)
                for (int h = 0; h < pc.Height; h++)
                {
                    bmp.SetPixel(w, h, pc.GetColor(pc[w, h]));
                }

            bmp.Save(@"C:\temp\1.bmp");
        }
    }
}
