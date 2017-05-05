using SquaredInfinity.Graphics.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Extensions
{
    public static partial class IPixelCanvasExtensions
    {
        public static void Save(this IPixelCanvas pc, string fullPath)
        {
            pc.Save(fullPath, new PngBitmapEncoder());
        }

        public static void Save(this IPixelCanvas pc, string fullPath, BitmapEncoder encoder)
        {
            var bmp = pc.ToFrozenWriteableBitmap();

            using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
                fs.Close();
            }
        }
    }
}
