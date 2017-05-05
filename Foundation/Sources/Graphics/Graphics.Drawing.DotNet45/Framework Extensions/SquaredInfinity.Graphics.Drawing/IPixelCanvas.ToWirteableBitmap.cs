using SquaredInfinity.Graphics.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Extensions
{
    public static partial class IPixelCanvasExtensions
    {
        public static WriteableBitmap ToWriteableBitmap(this IPixelCanvas pc)
        {
            var wb = new WriteableBitmap(pc.Width, pc.Height, 96, 96, PixelFormats.Pbgra32, palette: null);

            var bounds = new Int32Rect(0, 0, pc.Width, pc.Height);

            wb.WritePixels(bounds, pc.GetPixels(), pc.Stride, 0);

            return wb;
        }

        public static WriteableBitmap ToFrozenWriteableBitmap(this IPixelCanvas pc)
        {
            var wb = pc.ToWriteableBitmap();

            wb.Freeze();

            return wb;
        }
    }
}
