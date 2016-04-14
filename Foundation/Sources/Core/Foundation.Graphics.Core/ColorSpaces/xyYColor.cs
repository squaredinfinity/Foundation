using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Graphics.ColorSpaces
{
    public class xyYColor : ColorBase
    {
        public ColorChannel Alpha { get; private set; }
        public ColorChannel x { get; private set; }
        public ColorChannel y { get; private set; }
        public ColorChannel Y { get; private set; }

        public xyYColor(Double alpha, double x, double y, double Y)
            : base(KnownColorSpaces.xyY)
        {
            this.Alpha = new ColorChannel(KnownColorSpaces.xyY.Alpha, alpha);
            this.x = new ColorChannel(KnownColorSpaces.xyY.x, x);
            this.y = new ColorChannel(KnownColorSpaces.xyY.y, y);
            this.Y = new ColorChannel(KnownColorSpaces.xyY.Y, Y);

            this.Channels = new ColorChannelCollection(Alpha, this.x, this.y, this.Y);
        }
    }
}
