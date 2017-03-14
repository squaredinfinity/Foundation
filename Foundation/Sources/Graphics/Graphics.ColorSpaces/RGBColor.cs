using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Graphics.ColorSpaces
{
    public abstract class RGBColor : ColorBase
    {
        public ColorChannel Alpha { get; set; }
        public ColorChannel R { get; set; }
        public ColorChannel G { get; set; }
        public ColorChannel B { get; set; }

        public RGBColor(double alpha, double r, double g, double b, RGBColorSpace colorSpace)
            : base(colorSpace)
        {
            this.Alpha = new ColorChannel(colorSpace.Alpha, alpha);
            this.R = new ColorChannel(colorSpace.R, r);
            this.G = new ColorChannel(colorSpace.G, g);
            this.B = new ColorChannel(colorSpace.B, b);

            this.Channels = new ColorChannelCollection(Alpha, R, G, B);
        }
    }
}
