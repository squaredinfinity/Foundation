using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Graphics.ColorSpaces
{
    public class LabColor : ColorBase
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
            this.a = new ColorChannel(KnownColorSpaces.Lab.a, a);
            this.b = new ColorChannel(KnownColorSpaces.Lab.b, b);

            // NOTE:    L should be in inclusive range of [0,100]
            //          some calculations may push it a little bit over 100 due to rounding errors
            //          in this implementation we will cut it at 100 exactly if that is a case
            if(L.IsGreaterThan(100.00))
            {
                L = 100.00;
            }

            this.L = new ColorChannel(KnownColorSpaces.Lab.Lightness, L);

            this.Channels = new ColorChannelCollection(Alpha, this.L, this.a, this.b);
        }
    }
}
