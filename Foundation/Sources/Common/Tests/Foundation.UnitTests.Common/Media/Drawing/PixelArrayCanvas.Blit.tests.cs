using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SquaredInfinity.Foundation.Extensions;
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
    public class PixelArrayCanvas__Blit
    {
        [TestMethod]
        public void Blit()
        {
            var pc_1 = new PixelArrayCanvas(10, 10);
            pc_1.DrawLineDDA(1, 1, 9, 9, System.Windows.Media.Colors.DeepPink);

            var pc_2 = new PixelArrayCanvas(10, 10);
            pc_2.DrawLineDDA(1, 9, 9, 1, System.Windows.Media.Colors.DeepPink);

            var bmp = pc_1.ToFrozenWriteableBitmap();

            using (FileStream fs = new FileStream(@"c:\temp\1.bmp", FileMode.OpenOrCreate))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
                fs.Close();
            }


            bmp = pc_2.ToFrozenWriteableBitmap();

            using (FileStream fs = new FileStream(@"c:\temp\2.bmp", FileMode.OpenOrCreate))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
                fs.Close();
            }

            pc_1.Blit(new System.Drawing.Rectangle(0, 0, 10, 10), pc_2, new System.Drawing.Rectangle(0,0,10,10), 255, 255, 255, 255, BlendMode.Alpha);

            bmp = pc_1.ToFrozenWriteableBitmap();

            using (FileStream fs = new FileStream(@"c:\temp\3.bmp", FileMode.OpenOrCreate))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
                fs.Close();
            }


            

            //bmp.Save(@"C:\temp\1.bmp");
        }
    }
}
