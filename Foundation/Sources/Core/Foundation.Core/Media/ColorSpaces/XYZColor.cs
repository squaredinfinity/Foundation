using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Media.ColorSpaces
{
    public class XYZColor : Color
    {
        public ColorChannel Alpha { get; private set; }
        public ColorChannel X { get; private set; }
        public ColorChannel Y { get; private set; }
        public ColorChannel Z { get; private set; }

        public XYZColor(double alpha, double x, double y, double z)
            : this(KnownColorSpaces.XYZ, alpha, x, y, z)
        {
        }

        internal XYZColor(XYZColorSpace colorSpace, double alpha, double x, double y, double z)
            : base(colorSpace)
        {
            this.Alpha = new ColorChannel(colorSpace.Alpha, alpha);
            this.X = new ColorChannel(colorSpace.X, x);
            this.Y = new ColorChannel(colorSpace.Y, y);
            this.Z = new ColorChannel(colorSpace.Z, z);

            this.Channels = new ColorChannelCollection(Alpha, X, Y, Z);
        }
    }
}
