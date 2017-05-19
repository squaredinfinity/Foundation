using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths
{
    public class LineSegment2D
    {
        Point2D _a;
        public Point2D A => _a;

        Point2D _b;
        public Point2D B => _b;

        Slope2D _slope;
        Slope2D Slope => _slope;

        double _length;
        public double Length => _length;

        #region Constructors

        public LineSegment2D(double x1, double y1, double x2, double y2)
        {
            _slope = new Slope2D(x1, y1, x2, y2);

            _a = new Point2D(x1, y1);
            _b = new Point2D(x2, y2);

            _length = _a.Distance(_b);
        }

        public LineSegment2D(Point2D a, Point2D b)
        {
            _slope = new Slope2D(a.X, a.Y, b.X, b.Y);

            _a = a;
            _b = b;

            _length = a.Distance(b);
        }

        #endregion

        /// <summary>
        /// Returns the shortest distence between this segment and given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double Distance(Point2D p)
        {
            // using https://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line#Line_defined_by_two_points
            // the numerator is twice the area of the triangle with its vertices at A, B and P
            // the simplified form of that area is 
            //
            // 1/2 * |(a.X - p.X)(b.Y-a.Y) - (a.X - b.X)(p.Y - a.Y)|
            //
            // as described here: https://en.wikipedia.org/wiki/Area_of_a_triangle#Using_coordinates
            //
            // we then use Aera and base (AB) to calculate height of the triangle as
            //
            // h = 2 * (A/b)

            var h =
                ((A.X - p.X) * (B.Y - A.Y) - (A.X - B.X) * (p.Y - A.Y)) // numerator can be negative, we'll abs the result later
                /
                _length; // length is alway positive

            return Math.Abs(h);
        }
    }
}
