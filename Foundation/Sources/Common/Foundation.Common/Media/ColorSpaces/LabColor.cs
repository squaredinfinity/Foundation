using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Media.ColorSpaces
{
    public class LabColor : Color
    {
        public ColorChannel Alpha { get; private set; }
        /// <summary>
        /// [0..100]
        /// </summary>
        public ColorChannel L { get; private set; }
        public ColorChannel a { get; private set; }
        public ColorChannel b { get; private set; }

        public LabColor(double alpha, double L, double a, double b)
            : base(KnownColorSpaces.Lab)
        {
            this.Alpha = new ColorChannel(KnownColorSpaces.Lab.Alpha, alpha);
            this.L = new ColorChannel(KnownColorSpaces.Lab.Lightness, L % 100);
            this.a = new ColorChannel(KnownColorSpaces.Lab.a, a);
            this.b = new ColorChannel(KnownColorSpaces.Lab.b, b);

            this.Channels = new ColorChannelCollection(Alpha, this.L, this.a, this.b);
        }
    }
}
