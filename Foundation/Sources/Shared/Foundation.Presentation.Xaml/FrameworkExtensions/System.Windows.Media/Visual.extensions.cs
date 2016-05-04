using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Foundation.Extensions
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

            var bounds = VisualTreeHelper.GetDescendantBounds(visual);

            return visual.RenderToBitmap(bounds.Size, new Point(0, 0), bounds.Size, pixelFormat);
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

            var dv = new DrawingVisual();

            // size visual if needed
            var fElement = visual as FrameworkElement;
            var old_size = (Size?)null;

            if (fElement != null)
            {
                if(!fElement.ActualHeight.IsCloseTo(visualSize.Height) || !fElement.ActualWidth.IsCloseTo(visualSize.Width))
                {
                    old_size = new Size(fElement.Width, fElement.Height);

                    fElement.Width = visualSize.Width;
                    fElement.Height = visualSize.Height;
                    fElement.Arrange(new Rect(visualSize));
                    fElement.UpdateLayout();
                }
            }

            using (var ctx = dv.RenderOpen())
            {
                var vb = new VisualBrush(visual);

                if (flowDirection == FlowDirection.RightToLeft)
                {
                    var transformGroup = new TransformGroup();

                    transformGroup.Children.Add(new ScaleTransform(-1, 1));

                    transformGroup.Children.Add(new TranslateTransform(visualSize.Width - 1, 0));

                    ctx.PushTransform(transformGroup);
                }

                ctx.DrawRectangle(vb, null, new Rect(visualPositionOnBitmap, visualSize));
            }

            rtb.Render(dv);

            if (fElement != null && old_size != null)
            {
                fElement.Width = old_size.Value.Width;
                fElement.Height = old_size.Value.Height;
                fElement.Arrange(new Rect(new Size));
                fElement.UpdateLayout();
            }

            return rtb;
        }
    }
}
