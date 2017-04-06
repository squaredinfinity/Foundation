using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{
    /// <summary>
    /// y = mx+b
    /// </summary>
    public struct Line2D
    {
        double _b;
        public double B { get { return _b; } }

        Slope _slope;
        Slope Slope { get { return _slope; } }

        public double M { get { return _slope.M; } }

        public Line2D(double m, double b)
        {
            _b = b;
            _slope = new Slope(m);
        }

        public Line2D(Slope slope, double b)
        {
            _b = b;
            _slope = slope;
        }

        public Line2D(double x1, double y1, double x2, double y2)
        {
            _slope = new Slope(x1, y1, x2, y2);
            _b = y1 - _slope.M * x1;
        }

        //public Line2D GetPerpendicularLine()
        //{
            
        //}
    }
}
