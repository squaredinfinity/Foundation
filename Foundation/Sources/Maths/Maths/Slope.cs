﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Maths
{
    public struct Slope
    {
        double _m;
        public double M { get { return _m; } }

        public Slope(double m)
        {
            this._m = m;
        }

        public Slope(double x1, double y1, double x2, double y2)
        {
            _m = (y2 - y1) / (x2 - x1);
        }

        public static bool AreParallel(Slope a, Slope b)
        {
            return a._m.IsCloseTo(b._m);
        }

        public static bool ArePerpendicular(Slope a, Slope b)
        {
            return (a._m * b._m).IsCloseTo(-1.0);
        }

        public Slope GetPerpendicularSlope()
        {
            return (-1.0 / _m);
        }

        public static implicit operator Slope(double m)
        {
            return new Slope(m);
        }

        public static implicit operator double(Slope slope)
        {
            return slope._m;
        }
    }
}