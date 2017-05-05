using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Extensions
{
    public static class VisualExtensions
    {
        public static BitmapSource RenderToBitmap(
            this Visual visual,
            FlowDirection flowDirection = FlowDirection.LeftToRight,
            double dpiX = 96.0,
            double dpiY = 96.0)
        {
            return visual.RenderToBitmap(PixelFormats.Pbgra32, flowDirection, dpiX, dpiY);
        }

        public static BitmapSource RenderToBitmap(
            this Visual visual,
            Size size,
            FlowDirection flowDirection = FlowDirection.LeftToRight,
            double dpiX = 96.0,
            double dpiY = 96.0)
        {
            if (visual == null)
                throw new ArgumentException("visual");

            return visual.RenderToBitmap(size, new Point(0, 0), size, PixelFormats.Pbgra32);
        }

        public static BitmapSource RenderToBitmap(
            this Visual visual,
            PixelFormat pixelFormat,
            FlowDirection flowDirection = FlowDirection.LeftToRight,
            double dpiX = 96.0,
            double dpiY = 96.0)
        {
            if (visual == null)
                throw new ArgumentException("visual");

            var cv = visual as ContainerVisual;
            if (cv != null)
            {
                var bounds = cv.DescendantBounds;

                var size = new Size(bounds.Right, bounds.Bottom);

                return visual.RenderToBitmap(size, bounds.Location, bounds.Size, pixelFormat);
            }
            else
            {
                var bounds = VisualTreeHelper.GetDescendantBounds(visual);

                var size = new Size(bounds.Right, bounds.Bottom);

                return visual.RenderToBitmap(size, bounds.Location, bounds.Size, pixelFormat);
            }
        }

        public static BitmapSource RenderToBitmap(
            this Visual visual,
            Size bitmapSize,
            Point visualPositionOnBitmap,
            FlowDirection flowDirection = FlowDirection.LeftToRight,
            double dpiX = 96.0,
            double dpiY = 96.0)
        {
            if (visual == null)
                throw new ArgumentException("visual");

            var bounds = VisualTreeHelper.GetDescendantBounds(visual);

            return visual.RenderToBitmap(bitmapSize, visualPositionOnBitmap, bounds.Size, PixelFormats.Pbgra32);
        }


        public static BitmapSource RenderToBitmap(
            this Visual visual,
            Size bitmapSize,
            Point visualPositionOnBitmap,
            Size visualSize,
            FlowDirection flowDirection = FlowDirection.LeftToRight,
            double dpiX = 96.0,
            double dpiY = 96.0)
        {
            return visual.RenderToBitmap(bitmapSize, visualPositionOnBitmap, visualSize, PixelFormats.Pbgra32);
        }

        /// <summary>
        /// Creates a bit map representation of a given visual
        /// </summary>
        /// <param name="visual">visual to render</param>
        /// <param name="bitmapSize">the size of bitmap (this may be larger then visual itself, but normally size of the visual)</param>
        /// <param name="visualPositionOnBitmap">position of visual on bitmap (normally 0,0)</param>
        /// <param name="visualSize">size of the visual</param>
        /// <param name="pixelFormat"></param>
        /// <param name="flowDirection"></param>
        /// <param name="dpiX"></param>
        /// <param name="dpiY"></param>
        /// <returns></returns>
        public static BitmapSource RenderToBitmap(
            this Visual visual,
            Size bitmapSize,
            Point visualPositionOnBitmap,
            Size visualSize,
            PixelFormat pixelFormat,
            FlowDirection flowDirection = FlowDirection.LeftToRight,
            double dpiX = 96.0,
            double dpiY = 96.0)
        {
            var rtb = new RenderTargetBitmap((int)(bitmapSize.Width * dpiX / 96.0),
                                             (int)(bitmapSize.Height * dpiY / 96.0),
                                             dpiX,
                                             dpiY,
                                             pixelFormat);

            var dv = visual as DrawingVisual;

            if (dv != null)
            {
                var transform = new TranslateTransform(-dv.ContentBounds.Left,-dv.ContentBounds.Top);
                transform.Freeze();

                dv.Transform = transform;
                
                rtb.Render(dv);

                dv.Transform = null;
            }
            else
            {
                dv = new DrawingVisual();

                using (var ctx = dv.RenderOpen())
                {
                    ctx.Render(visual, visualPositionOnBitmap, visualSize, flowDirection);
                }

                rtb.Render(dv);
            }


            return rtb;
        }
    }
}
