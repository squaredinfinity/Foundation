using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Extensions
{
    public static class BitmapSourceExtensions
    {
        //static readonly DependencyProperty MemoryUsageProperty =
        //    DependencyProperty.RegisterAttached("MemoryUsage", typeof(GCMemoryUsagePressure),
        //    typeof(BitmapSourceExtensions), new PropertyMetadata(null));

        ///// <summary>
        ///// Reports the memory used by <paramref name="bitmap"/> to the GC.
        ///// </summary>
        ///// <param name="bitmap">The bitmap.</param>
        //public static void AddMemoryPressure(this BitmapSource bitmap)
        //{
        //    if (bitmap == null)
        //        return;

        //    // "stride" is bytes used per bitmap scanline

        //    int stride = bitmap.PixelWidth * ((bitmap.Format.BitsPerPixel + 7) / 8);

        //    int height = bitmap.PixelHeight;

        //    bitmap.SetValue(MemoryUsageProperty, new GCMemoryUsagePressure((long)stride * height));
        //}

        public static void Save(this BitmapSource bitmap, string path)
        {
            bitmap.Save(path, new PngBitmapEncoder());
        }

        public static void Save(this BitmapSource bitmap, string path, BitmapEncoder encoder)
        {
            using (var fs = File.OpenWrite(path))
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fs);
            }
        }

        public static byte[] GetPixels(this BitmapSource bitmap)
        {
            return bitmap.GetPixels(bitmap.PixelWidth * 4);
        }

        public static byte[] GetPixels(this BitmapSource bitmap, int stride)
        {
            var result = new byte[bitmap.PixelHeight * stride];

            bitmap.CopyPixels(result, stride, 0);

            return result;
        }

        public static Metafile CreateMetafile(
            this BitmapSource bitmap, 
            double horizontalScale = 1.0,
            double verticalScale = 1.0,
            EmfType emf = EmfType.EmfOnly,
            SmoothingMode smoothingMode = SmoothingMode.HighQuality,
            InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic,
            PixelOffsetMode pixelOffsetMode = PixelOffsetMode.HighQuality,
            CompositingQuality compositingQuality = CompositingQuality.HighQuality)
        {
            var mf = (Metafile)null;

            using (System.Drawing.Graphics cx = System.Drawing.Graphics.FromHwndInternal(IntPtr.Zero))
            {
                mf = new Metafile(new MemoryStream(), cx.GetHdc(), emf);

                using (var g = System.Drawing.Graphics.FromImage(mf))
                {
                    var img = bitmap.ToBitmap();

                    g.SmoothingMode = smoothingMode;
                    g.InterpolationMode = interpolationMode;
                    g.PixelOffsetMode = pixelOffsetMode;
                    g.CompositingQuality = compositingQuality;

                    var rect =
                        new System.Drawing.RectangleF(
                            0,
                            0,
                            img.PhysicalDimension.Width * (float)horizontalScale,
                            img.PhysicalDimension.Height * (float)verticalScale);

                    g.DrawImage(img, rect);
                }
            }

            return mf;
        }
    }
}
