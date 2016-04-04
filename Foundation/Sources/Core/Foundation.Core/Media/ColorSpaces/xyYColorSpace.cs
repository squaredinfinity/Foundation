using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Media.ColorSpaces
{
    public class xyYColorSpace : IColorSpace
    {
        public string Name
        {
            get { return "CIE xyY"; }
        }

        public ColorChannelDefinition Alpha { get; private set; }
        public ColorChannelDefinition x { get; private set; }
        public ColorChannelDefinition y { get; private set; }
        public ColorChannelDefinition Y { get; private set; }

        internal xyYColorSpace()
        {
            Alpha = new ColorChannelDefinition(this, "Alpha", "A");
            x = new ColorChannelDefinition(this, "x", "x");
            x = new ColorChannelDefinition(this, "y", "y");
            Y = new ColorChannelDefinition(this, "Y", "Y");
        }

        public IColor FromXYZColor(XYZColor xyzColor)
        {
            var x = xyzColor.X / (xyzColor.X + xyzColor.Y + xyzColor.Z);
            var y = xyzColor.Y / (xyzColor.X + xyzColor.Y + xyzColor.Z);

            return new xyYColor(xyzColor.Alpha, x, y, xyzColor.Y);
        }

        public XYZColor ToXYZColor(IColor color)
        {
            var xyYColor = color as xyYColor;

            var X = xyYColor.x * (xyYColor.Y / xyYColor.y);
            var Z = (1 - xyYColor.x - xyYColor.y) * (xyYColor.Y / xyYColor.y);

            return new XYZColor(xyYColor.Alpha, X, xyYColor.Y, Z);
        }
    }
}
