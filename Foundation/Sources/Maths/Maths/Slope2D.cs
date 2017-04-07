using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Maths
{
    public struct Slope2D
    {
        double _m;
        public double M { get { return _m; } }

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
