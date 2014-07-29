using SquaredInfinity.Foundation.Media.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ColorExtensions
    {
        public static ScRBGColor ToScRGBColor(this System.Windows.Media.Color color)
        {
            return new ScRBGColor(color.ScA, color.ScR, color.ScG, color.ScB);
        }

        public static System.Windows.Media.Color ToWindowsMediaColor(this ScRBGColor color)
        {
            return System.Windows.Media.Color.FromScRgb((float)color.Alpha, (float)color.R, (float)color.G, (float)color.B);
        }
    }
}
