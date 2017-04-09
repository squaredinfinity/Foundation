using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Maths
{
    /// <summary>
    /// Represents a slope or gradient of a straight line
    /// https://en.wikipedia.org/wiki/Slope
    /// </summary>
    public struct Slope2D
    {
        public static readonly Slope2D Horisontal = new Slope2D(0);
        public static readonly Slope2D Vertical = new Slope2D(double.NaN);

        double _m;
        public double M { get { return _m; } }

        /// <summary>
        /// True if the straight line with this slope is increasing, i.e. m > 0
        /// </summary>
        public bool IsIncreasing
        {
            get { return _m > 0; }
        }

        /// <summary>
        /// True if the straight line with this slope is increasing, i.e. 0 > m
        /// </summary>
        public bool IsDecreasing
        {
            get { return _m < 0; }
        }

        /// <summary>
        /// True if straight line with this line is a horisontal line, i.e. m == 0
        /// </summary>
        public bool IsHorisontal
        {
            get { return _m == 0; }
        }

        /// <summary>
        /// True if straight line with this slope is a vertical line, i.e. m is undefined
        /// </summary>
        public bool IsVertical
        {
            get { return _m == double.NaN; }
        }


        public Slope2D(double m)
        {
            this._m = m;
        }

        public Slope2D(double x1, double y1, double x2, double y2)
        {
            _m = (y2 - y1) / (x2 - x1);
        }

        public static bool AreParallel(Slope2D a, Slope2D b)
        {
            return a._m.IsCloseTo(b._m);
        }

        public static bool ArePerpendicular(Slope2D a, Slope2D b)
        {
            return (a._m * b._m).IsCloseTo(-1.0);
        }

        public Slope2D GetPerpendicularSlope()
        {
            return (-1.0 / _m);
        }

        public static implicit operator Slope2D(double m)
        {
            return new Slope2D(m);
        }

        public static implicit operator double(Slope2D slope)
        {
            return slope._m;
        }
    }
}
