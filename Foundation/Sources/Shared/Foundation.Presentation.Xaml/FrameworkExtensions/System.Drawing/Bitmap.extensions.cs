using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class BitmapExtensions
    {
        public static byte[] ToArray(this Bitmap bitmap)
        {
            return bitmap.ToArray(ImageFormat.Png);
        }

        public static byte[] ToArray(this Bitmap bitmap, ImageFormat format)
        {
            var bytes = new byte[0];

            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, format);
                stream.Close();

                bytes = stream.ToArray();
            }

            return bytes;
        }
    }
}
