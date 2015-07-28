using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class BitmapSourceExtensions
    {
        static readonly DependencyProperty MemoryUsageProperty =
            DependencyProperty.RegisterAttached("MemoryUsage", typeof(GCMemoryUsagePressure),
            typeof(BitmapSourceExtensions), new PropertyMetadata(null));

        /// <summary>
        /// Reports the memory used by <paramref name="bitmap"/> to the GC.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public static void AddMemoryPressure(this BitmapSource bitmap)
        {
            if (bitmap == null)
                return;

            // "stride" is bytes used per bitmap scanline

            int stride = bitmap.PixelWidth * ((bitmap.Format.BitsPerPixel + 7) / 8);

            int height = bitmap.PixelHeight;

            bitmap.SetValue(MemoryUsageProperty, new GCMemoryUsagePressure((long)stride * height));
        }
    }
}
