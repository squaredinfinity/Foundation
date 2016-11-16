using SquaredInfinity.Foundation.Graphics.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ColorExtensions
    {
        public static ScRGBColor ToScRGBColor(this System.Windows.Media.Color color)
        {
            return new ScRGBColor(color.ScA, color.ScR, color.ScG, color.ScB);
        }

        public static System.Windows.Media.Color ToWindowsMediaColor(this ScRGBColor color)
        {
            return System.Windows.Media.Color.FromScRgb((float)color.Alpha, (float)color.R, (float)color.G, (float)color.B);
        }

        public static System.Windows.Media.Color ToWindowsMediaColor(this LabColor color)
        {
            var xyz = KnownColorSpaces.Lab.ToXYZColor(color) as XYZColor;

            var scrgb = KnownColorSpaces.scRGB.FromXYZColor(xyz) as ScRGBColor;

            return System.Windows.Media.Color.FromScRgb((float)scrgb.Alpha, (float)scrgb.R, (float)scrgb.G, (float)scrgb.B);
        }

        /// <summary>
        /// Returns new color based on existing color with modified lightness component.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="lightnessMultiplier">0.5 to reduce lightness by 50%, 1.5 to increase lightness by 50%</param>
        /// <returns></returns>
        public static System.Windows.Media.Color ChangeLighthness(this System.Windows.Media.Color color, double lightnessMultiplier)
        {
            var scRGB = color.ToScRGBColor();
            var xyz = KnownColorSpaces.scRGB.ToXYZColor(scRGB);
            var lab = (LabColor) KnownColorSpaces.Lab.FromXYZColor(xyz);

            var newColor = new LabColor(lab.Alpha, lab.L * lightnessMultiplier, lab.a, lab.b);

            return newColor.ToWindowsMediaColor();
        }

        public static System.Windows.Media.Color ChangeAlpha(this System.Windows.Media.Color color, byte new_alpha)
        {
            return System.Windows.Media.Color.FromArgb(new_alpha, color.R, color.G, color.B);
        }

        public static System.Windows.Media.Color ChangeAlpha(this System.Windows.Media.Color color, double new_alpha)
        {
            return System.Windows.Media.Color.FromArgb((byte)(color.A * new_alpha), color.R, color.G, color.B);
        }
    }
}
