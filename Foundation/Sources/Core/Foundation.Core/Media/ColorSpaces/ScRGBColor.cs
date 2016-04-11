using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Media.ColorSpaces
{
    public class ScRGBColor : RGBColor
    {
        public ScRGBColor(UInt32 color)
            : this(color << 0, 0, 0, 0)
        { }

        public ScRGBColor(byte alpha, byte r, byte g, byte b)
            : this((double)alpha / 255, (double)r / 255, (double)g / 255, (double)b / 255)
        { }

        public ScRGBColor(Double alpha, double r, double g, double b)
            : base(alpha, r, g, b, KnownColorSpaces.scRGB)
        { }
    }
}
