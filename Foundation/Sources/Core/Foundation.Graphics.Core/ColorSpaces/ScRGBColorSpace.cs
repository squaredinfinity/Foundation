using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Graphics.ColorSpaces
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/ScRGB (D65)
    /// </summary>
    public class scRGBColorSpace : RGBColorSpace
    {
        public override string Name
        {
            get { return "scRGB (D65)"; }
        }

        protected override double[,] M_XYZToRGB
        {
            get
            {
                return new double[3, 3]
                {
                    {  3.2404542, -1.5371385, -0.4985314 },
                    { -0.9692660,  1.8760108,  0.0415560 },
                    {  0.0556434, -0.2040259,  1.0572252 }
                };
            }
        }

        protected override double[,] M_RGBToXYZ
        {
            get 
            {
                return new double[3, 3]
                {
                    { 0.4124564, 0.3575761, 0.1804375  },
                    { 0.2126729, 0.7151522, 0.0721750  },
                    { 0.0193339, 0.1191920,  0.9503041 }
                };
            }
        }

        protected override IColor GetColor(double alpha, double red, double green, double blue)
        {
            return new ScRGBColor(alpha, red, green, blue);
        }
    }
}
