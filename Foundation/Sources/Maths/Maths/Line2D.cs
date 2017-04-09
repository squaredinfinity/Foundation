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

        Slope2D _slope;
        Slope2D Slope { get { return _slope; } }

        public double M { get { return _slope.M; } }

        public Line2D(double m)
            : this(m, 0)
        { }

        public Line2D(double m, double b)
        {
            _b = b;
            _slope = new Slope2D(m);
        }

        public Line2D(Slope2D slope, double b)
        {
            _b = b;
            _slope = slope;
        }

        public Line2D(double x1, double y1, double x2, double y2)
        {
            _slope = new Slope2D(x1, y1, x2, y2);
            _b = y1 - _slope.M * x1;
        }

        /// <summary>
        /// Calculates y given specified x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetY(double x)
        {
            return _slope.M * x + _b;
        }

        /// <summary>
        /// Calculates y for a specified line and x
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double GetY(double m, double b, double x)
        {
            return m * x + b;
        }

        /// <summary>
        /// Calculates x given specified y
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetX(double y)
        {
            return (y - _b) / _slope.M;
        }

        /// <summary>
        /// Calculates y for a specified line and y
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double GetX(double m, double b, double y)
        {
            return (y - b) / m;
        }

        //public Line2D GetPerpendicularLine()
        //{

        //}
    }
}
