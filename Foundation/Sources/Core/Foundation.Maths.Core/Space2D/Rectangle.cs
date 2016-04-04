using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Space2D
{
    public struct Rectangle : IEquatable<Rectangle>
    {
        public static readonly Rectangle Empty = new Rectangle();

        double _x;
        public double X {  get { return _x; } set { _x = value; } }
        double _y;
        public double Y {  get { return _y; } set { _y = value; } }

        double _width;
        public double Width {  get { return _width; } set { _width = value; } }
        double _height;
        public double Height {  get { return _height; } set { _height = value; } }


        public double Left { get { return _x; } }
        public double Right {  get { return _x + _width; } }
        public double Top { get { return _y; } set { _y = value; } }
        public double Bottom {  get { return _y + _height; } }

        public bool IsEmpty { get { return _width <= 0 || _height <= 0; } }

        public Rectangle(double x, double y, double width, double height)
        {
            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
        }

        #region Contains

        public bool Contains(Rectangle other)
        {
            if (IsEmpty || other.IsEmpty)
                return false;

            return
                _x <= other._x
                &&
                _y <= other._y
                &&
                // Right, access fields directly for performance
                _x + _width >= other._x + other._width
                &&
                // Bottom, access fields directly for performance
                _y + _height >= other._y + other._height;
        }

        #endregion

        #region Intersection

        public bool IntersectsWith(Rectangle other)
        {
            if (IsEmpty || other.IsEmpty)
                return false;

            return
                // other.Left <= Right
                other._x <= _x + _width
                &&
                // other.Right >= Left
                other._x + other._width >= _x
                &&
                // other.Top <= Bottom
                other._y <= _y + _height
                &&
                // other.Bottom >= Top
                other._y + other._height >= _y;
        }

        #endregion

        #region Union

        public static Rectangle Union(Rectangle r1, Rectangle r2)
        {
            var x = Math.Min(r1._x, r2._x);
            var y = Math.Min(r1._y, r2._y);

            return new Rectangle(
                x,
                y,
                Math.Max(r1._x + r1._width, r1._x + r2._width) - x,
                Math.Max(r1._y + r1._height, r2._y + r2._height) - y);
        }

        #endregion

        #region Offset

        public void Offset(double dx, double dy)
        {
            _x += dx;
            _y += dy;
        }

        #endregion

        #region Equality + Hash Code

        public override int GetHashCode()
        {
            return
                _x.GetHashCode()
                ^
                _y.GetHashCode()
                ^
                _width.GetHashCode()
                ^
                _height.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (other == null || !(other is Rectangle))
                return false;

            return Equals((Rectangle)other);
        }

        public bool Equals(Rectangle other)
        {
            return
                _x.Equals(other._x)
                &&
                _y.Equals(other._y)
                &&
                _width.Equals(other._width)
                &&
                _height.Equals(other._height);
        }

        #endregion
    }
}
