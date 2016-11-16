using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ImageSourceExtensions
    {
        /// <summary>
        /// Converts System.Windows.Media.ImageSource to System.Drawing.Bitmap.
        /// </summary>
        /// <param name="imageSource"></param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this ImageSource imageSource)
        {
            MemoryStream ms = new MemoryStream();

            var encoder = new BmpBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(imageSource as BitmapSource));

            encoder.Save(ms);

            ms.Flush();

            var bitmap = System.Drawing.Image.FromStream(ms) as Bitmap;

            return bitmap;
        }
    }
}
