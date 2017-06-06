using SquaredInfinity.Collections;
using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.DigitalSignalProcessing.PolygonSimplification
{
    /// <summary>
    /// Simplifies a 2D polygon based on specifed minimum distance between points.
    /// Any two points with distance less that minAllowedDistance will be simplified to just a single point.
    /// It preserves original points of the polygon.
    /// If two points are close and can be reduced, one of them will be removed and the other preserved.
    /// </summary>
    public class RadialDistancePolygon2DIndexesSimplification : Polygon2DIndexesSimplification
    {
        double _minAllowedDistance;
        public double MinAllowedDistance => _minAllowedDistance;

        public RadialDistancePolygon2DIndexesSimplification()
            : this(2.0) { }

        /// <summary>
        /// </summary>
        /// <param name="minAllowedDistance"></param>
        public RadialDistancePolygon2DIndexesSimplification(double minAllowedDistance)
        {
            _minAllowedDistance = minAllowedDistance;
        }

        public override IIndexFilter<T> SimplifyIndexes<T>(
            IReadOnlyList<T> points,
            int start_ix,
            int end_ix,
            Func<T, Point2D> getPoint)
        {
            // calculate how many points will there be in total
            var max_count = end_ix - start_ix + 1;

            if (max_count <= 2)
            {
                // if no more than 2 points
                // then there's nothing to simplify
                return IndexFilter<T>.AllIncluded(points);
            }

            var result = IndexFilter<T>.AllExcluded(points);

            var last = getPoint(points[start_ix]);

            // always include first item
            result.AddIndex(start_ix);

            for (int i = 1; i < max_count - 1; i++)
            {
                var current = getPoint(points[i]);

                if (last.Distance(current) < MinAllowedDistance)
                    continue;

                last = current;
                result.AddIndex(i);
            }

            // always include last item
            result.AddIndex(end_ix);

            return result;
        }
    }
}
