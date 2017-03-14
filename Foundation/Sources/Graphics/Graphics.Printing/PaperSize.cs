using SquaredInfinity.Foundation.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Graphics.Priniting
{
    public class PaperSize
    {
        public double WidthMM { get; private set; }
        public double HeightMM { get; private set; }

        public PaperSize(GraphicsUnits units, double width, double height)
        {
            if (units == GraphicsUnits.Centimiters)
            {
                WidthMM = width * 10;
                HeightMM = height * 10;
            }
            else if (units == GraphicsUnits.Milimeters)
            {
                WidthMM = width;
                HeightMM = height;
            }
            else
            {
                throw new NotSupportedException(nameof(units));
            }
        }
    }
}
