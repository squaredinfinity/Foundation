using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct Point3D : IEquatable<Point3D>
    {
        double _x;
        public double X { get { return _x; } set { _x = value; } }

        double _y;
        public double Y { get { return _y; } set { _y = value; } }

        double _z;
        public double Z { get { return _z; } set { _z = value; } }

        public Point3D(double x, double y, double z)
        {
            this._x = x;
            this._y = y;
            this._z = z;
        }

        #region Equality + Hash Code

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    _x.GetHashCode()
                    ^
                    _y.GetHashCode()
                    ^
                    _z.GetHashCode();
            }
        }

        public override bool Equals(object other)
        {
            if (other == null || !(other is Point3D))
                return false;

            return Equals((Point3D)other);
        }

        public bool Equals(Point3D other)
        {
            return
                _x.Equals(other._x)
                &&
                _y.Equals(other._y)
                &&
                _z.Equals(other._z);

        }

        #endregion

        public void Offset(double dx, double dy, double dz)
        {
            _x += dx;
            _y += dy;
            _z += dz;
        }

        public void Offset(Point2D point)
        {
            _x += point.X;
            _y += point.Y;
        }

        public void Offset(Point3D point)
        {
            _x += point._x;
            _y += point._y;
            _z += point._z;
        }

        public double Distance(Point3D other)
        {
            var sum = Math.Pow(other._x - _x, 2) + Math.Pow(other._y - _y, 2) + Math.Pow(other._z - _z, 2);

            var d = Math.Sqrt(sum);

            return d;
        }

        public string DebuggerDisplay
        {
            get { return $"{_x},{_y},{_z}"; }
        }
    }
}
