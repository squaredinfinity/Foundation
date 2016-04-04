using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Media.ColorSpaces
{
    public class ScRBGColor : RGBColor
    {
        public ScRBGColor(Double alpha, double r, double g, double b)
            : base(alpha, r, g, b, KnownColorSpaces.scRGB)
        { }
    }
}
