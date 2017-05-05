using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Extensions
{
    public static class GeometryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="bitmapSize"></param>
        /// <param name="geometryPositionOnBitmap"></param>
        /// <param name="brush">The Brush with which to fill the Geometry. This is optional, and can be null. If the brush is null, no fill is drawn.</param>
        /// <param name="pen">The Pen with which to stroke the Geometry. This is optional, and can be null. If the pen is null, no stroke is drawn</param>
        /// <param name="pixelFormat"></param>
        /// <param name="dpiX"></param>
        /// <param name="dpiY"></param>
        /// <returns></returns>
        public static BitmapSource RenderToBitmap(
            this Geometry geometry,
            Size bitmapSize,
            Point geometryPositionOnBitmap,
            Size geometrySize,
            Brush brush,
            Pen pen,
            PixelFormat pixelFormat,
            double dpiX = 96.0,
            double dpiY = 96.0)
        {
            var rtb = new RenderTargetBitmap((int)(bitmapSize.Width * dpiX / 96.0),
                                             (int)(bitmapSize.Height * dpiY / 96.0),
                                             dpiX,
                                             dpiY,
                                             pixelFormat);

            var dv = new DrawingVisual();

            

            using (var cx = dv.RenderOpen())
            {
                cx.Render(geometry, geometryPositionOnBitmap, brush, pen);
            }

            rtb.Render(dv);

            return rtb;
        }
    }
}
