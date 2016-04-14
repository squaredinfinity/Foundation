using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Graphics.ColorSpaces
{
    public static class KnownColorSpaces
    {
        static readonly XYZColorSpace xyz;
        public static XYZColorSpace XYZ { get { return xyz; } }

        static readonly xyYColorSpace xyy;
        public static xyYColorSpace xyY { get { return xyy; } }

        static readonly scRGBColorSpace scrgb;
        public static scRGBColorSpace scRGB { get { return scrgb; } }

        static readonly LabColorSpace lab;
        public static LabColorSpace Lab { get { return lab; } }

        static KnownColorSpaces()
        {
            xyz = new XYZColorSpace();
            xyy = new xyYColorSpace();
            scrgb = new scRGBColorSpace();
            lab = new LabColorSpace();
        }

        public static IColorSpace[] AllKnownColorSpaces()
        {
            return new IColorSpace[]
            {
                xyz,
                xyy,
                scrgb,
                lab,
            };
        }
    }
}
