using SquaredInfinity.Collections;
using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.DigitalSignalProcessing.PolygonSimplification
{
    public abstract class Polygon2DIndexesSimplification : IPolygon2DIndexesSimplification
    {
        public IIndexFilter<T> SimplifyIndexes<T>(
            IReadOnlyList<T> points,
            Func<T, Point2D> getPoint)
            => SimplifyIndexes(points, 0, points.Count - 1, getPoint);

        public abstract IIndexFilter<T> SimplifyIndexes<T>(
            IReadOnlyList<T> points,
            int start_ix,
            int end_ix,
            Func<T, Point2D> getPoint);
    }
}
