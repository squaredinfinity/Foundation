using System;
using SquaredInfinity.Extensions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct Rectangle : IEquatable<Rectangle>
    {
        //! NOTE:   Instance methods change this instance
        //          Static methods create a new instance

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

        public Point2D TopLeft
        {
            get { return new Point2D(_x, _y); }
        }

        public Point2D BottomLeft
        {
            get { return new Point2D(_x, _y + _height); }
        }

        public Point2D TopRight
        {
            get { return new Point2D(_x + _width, _y); }
        }

        public Point2D BottomRight
        {
            get { return new Point2D(_x + _width, _y + _height); }
        }

        public bool IsEmpty { get { return _width <= 0 || _height <= 0; } }

        public Rectangle(double x, double y, double width, double height)
        {
            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
        }

        #region Contains

        public bool Contains(Point2D point)
        {
            if (IsEmpty)
                return false;
            
            return
                _x <= point.X
                &&
                _y <= point.Y
                &&
                // Right, access fields directly for performance
                _x + _width <= point.X
                &&
                // Bottom, access fields directly for performance
                _y + _height <= point.Y;
        }

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

        public void Intersect(Rectangle other)
        {
            var x = Intersect(this, other);
            
            this._x = x._x;
            this._y = x._y;
            this._width = x._width;
            this._height = x._height;
        }

        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            if (a.IsEmpty || b.IsEmpty)
                return Rectangle.Empty;

            var x1 = Math.Max(a._x, b._x);
            var x2 = Math.Min(a._x + a._width, b._x + b._width);
            var y1 = Math.Max(a._y, b._y);
            var y2 = Math.Min(a._y + a._height, b._y + b._height);

            if (x2.IsGreaterThanOrClose(x1) && y2.IsGreaterThanOrClose(y1))
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);

            return Rectangle.Empty;
        }

        #endregion

        #region Union

        public void Union(Rectangle other)
        {
            var x = Union(this, other);

            this._x = x._x;
            this._y = x._y;
            this._width = x._width;
            this._height = x._height;
        }

        public static Rectangle Union(Rectangle r1, Rectangle r2)
        {
            if (r1.IsEmpty && r2.IsEmpty)
                return Rectangle.Empty;

            if (r1.IsEmpty)
                return r2;

            if (r2.IsEmpty)
                return r1;

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

        public void Offset(Point2D offset)
        {
            _x += offset.X;
            _y += offset.Y;
        }

        public void Offset(double dx, double dy)
        {
            _x += dx;
            _y += dy;
        }


        public static Rectangle Offset(Rectangle rect, Point2D offset)
        {
            return new Rectangle(rect._x += offset.X, rect._y += offset.Y, rect._width, rect._height);
        }

        public static Rectangle Offset(Rectangle rect, double dx, double dy)
        {
            return new Rectangle(rect._x += dx, rect._y += dy, rect._width, rect._height);
        }

        #endregion

        #region Clip

        public void Clip(Rectangle clip)
        {
            var x = Clip(this, clip);

            this._x = x._x;
            this._y = x._y;
            this._width = x._width;
            this._height = x._height;
        }

        /// <summary>
        /// Clips original rectangle to size of the clip rectangle.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static Rectangle Clip(Rectangle original, Rectangle clip)
        {
            var clipped = original.Clone();

            // left
            if (clip._x > clipped._x)
            {
                clipped._width -= clip._x - clipped._x;
                clipped._x = clip._x;
            }

            // top
            if (clip._y > clipped._y)
            {
                clipped._height -= clip._y - clipped._y;
                clipped._y = clip._y;
            }

            // right
            if (clip._x + clip._width < clipped._x + clipped._width)
                clipped._width = (clip._x + clip._width) - clipped._x;

            // bottom
            if (clip._y + clip._height < clipped._y + clipped._height)
                clipped._height = (clip._y + clip._height) - clipped._y;

            return clipped;
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
                _x.IsCloseTo(other._x)
                &&
                _y.IsCloseTo(other._y)
                &&
                _width.IsCloseTo(other._width)
                &&
                _height.IsCloseTo(other._height);
        }

        #endregion

        #region Operators

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }

        #endregion

        #region Clone

        // TODO: Is this still needed since Rectangle is now a Struct?
        public Rectangle Clone()
        {
            return new Rectangle(_x, _y, _width, _height);
        }

        #endregion

        public string DebuggerDisplay
        {
            get { return $"({X},{Y}), w:{Width}, h:{Height}"; }
        }
    }
}
