using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Graphics.Priniting
{
    public class MarginsSize
    {
        public static MarginsSize Normal { get; } = new MarginsSize(GraphicsUnits.Centimiters, 2.45, 2.45, 2.54, 2.54);
        public static MarginsSize Narrow { get; } = new MarginsSize(GraphicsUnits.Centimiters, 1.27, 1.27, 1.27, 1.27);
        public static MarginsSize Moderate { get; } = new MarginsSize(GraphicsUnits.Centimiters, 2.54, 2.54, 1.91, 1.91);
        public static MarginsSize Wide { get; } = new MarginsSize(GraphicsUnits.Centimiters, 2.54, 2.54, 5.08, 5.08);
        
        public double TopMM { get; private set; }
        public double BottomMM { get; private set; }

        public double LeftMM { get; private set; }

        public double RightMM { get; private set; }

        public MarginsSize(GraphicsUnits units, double top, double bottom, double left, double right)
        {
            if (units == GraphicsUnits.Centimiters)
            {
                this.TopMM = top * 10;
                this.BottomMM = bottom * 10;
                this.LeftMM = left * 10;
                this.RightMM = right * 10;
            }
            else if (units == GraphicsUnits.Milimeters)
            {
                this.TopMM = top;
                this.BottomMM = bottom;
                this.LeftMM = left;
                this.RightMM = right;
            }
            else
            {
                throw new NotSupportedException(nameof(units));
            }
        }
    }
}
