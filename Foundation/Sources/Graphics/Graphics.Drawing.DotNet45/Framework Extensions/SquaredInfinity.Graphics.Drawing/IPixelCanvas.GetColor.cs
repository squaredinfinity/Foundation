using SquaredInfinity.Graphics.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SquaredInfinity.Extensions
{
    public static partial class IPixelCanvasExtensions
    {
        public static int GetColor(this IPixelCanvas pc, System.Windows.Media.Color color)
        {
            return PixelCanvas.GetColor(color.A, color.R, color.G, color.B);
        }

        public static Color GetColor(this IPixelCanvas pc, int color)
        {
            var a = ((color >> 24) & 0xff);
            var r = ((color >> 16) & 0xff);
            var g = ((color >> 8) & 0xff);
            var b = ((color) & 0xff);

            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }
    }
}
