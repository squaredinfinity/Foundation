using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Presentation.Media.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    [TestClass]
    public class PixelCanvas__DrawLine
    {
        [TestMethod]
        public void DDA()
        {
            var pc = new WpfPixelCanvas(10, 10);
            pc.DrawLineDDA(1, 1, 9, 9, System.Windows.Media.Colors.DeepPink);

            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(10, 10);

            //for (int w = 0; w < pc.Width; w++)
            //    for (int h = 0; h < pc.Height; h++)
            //    {
            //        bmp.SetPixel(w, h, pc.GetColor(pc[w, h]));
            //    }

            var bmp = pc.ToFrozenWriteableBitmap();

            using (FileStream fs = new FileStream(@"c:\temp\2.bmp", FileMode.OpenOrCreate))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
                fs.Close();
            }


            

            //bmp.Save(@"C:\temp\1.bmp");
        }

        [TestMethod]
        public void DDA_Horizontal()
        {
            for (int i = 1; i < 9; i++)
            {
                var pc = new WpfPixelCanvas(10, 10);
                pc.DrawLineDDA(1, 0, i, 0, System.Windows.Media.Colors.DeepPink);

                var bmp = pc.ToFrozenWriteableBitmap();

                using (FileStream fs = new FileStream(@"c:\temp\DDA_Horizontal_{0}.bmp".FormatWith(i), FileMode.OpenOrCreate))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bmp));
                    encoder.Save(fs);
                    fs.Close();
                }
            }
        }
    }
}
