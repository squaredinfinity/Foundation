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
            PixelFormat pixelFormat,
        FlowDirection flowDirection = FlowDirection.LeftToRight,
        double dpiX = 96.0,
        double dpiY = 96.0)
        {
            if (visual == null)
                throw new ArgumentException("visual");

            var bounds = VisualTreeHelper.GetDescendantBounds(visual);

            var rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
                                             (int)(bounds.Height * dpiY / 96.0),
                                             dpiX,
                                             dpiY,
                                             pixelFormat);

            var dv = new DrawingVisual();

            using (var ctx = dv.RenderOpen())
            {
                var vb = new VisualBrush(visual);

                if (flowDirection == FlowDirection.RightToLeft)
                {
                    var transformGroup = new TransformGroup();

                    transformGroup.Children.Add(new ScaleTransform(-1, 1));

                    transformGroup.Children.Add(new TranslateTransform(bounds.Size.Width - 1, 0));

                    ctx.PushTransform(transformGroup);
                }

                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            return rtb;
        }
    }
}
