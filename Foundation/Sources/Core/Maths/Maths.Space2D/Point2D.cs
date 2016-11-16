using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Space2D
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct Point2D : IEquatable<Point2D>
    {
        double _x;
        public double X { get { return _x; } set { _x = value; } }

        double _y;
        public double Y { get { return _y; } set { _y = value; } }

        public Point2D(double x, double y)
        {
            this._x = x;
            this._y = y;
        }

        #region Equality + Hash Code

        public override int GetHashCode()
        {
            return
                _x.GetHashCode() 
                ^
                _y.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other == null || !(other is Point2D))
                return false;

            return Equals((Point2D) other);
        }

        public bool Equals(Point2D other)
        {
            return
                _x.Equals(other._x)
                &&
                _y.Equals(other._y);

        }

        #endregion

        public void Offset(int dx, int dy)
        {
            _x += dx;
            _y += dy;
        }

        public void Offset(Point2D point)
        {
            _x += point.X;
            _y += point.Y;
        }

        public double Distance(Point2D other)
        {
            // d = |x - y| = Sqrt(Sum((xi - yi)^2))

            var sum = Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2);

            var d = Math.Sqrt(sum);

            return d;
        }

        public string DebuggerDisplay
        {
            get { return $"{X},{Y}"; }
        }
    }
}
