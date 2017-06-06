using SquaredInfinity.Collections;
using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.DigitalSignalProcessing.PolygonSimplification
{
    public class CompositePolygon2DIndexesSimplification : Polygon2DIndexesSimplification
    {
        readonly List<IPolygon2DIndexesSimplification> _children = new List<IPolygon2DIndexesSimplification>();
        public List<IPolygon2DIndexesSimplification> Children => _children;

        public override IIndexFilter<T> SimplifyIndexes<T>(
            IReadOnlyList<T> points,
            int start_ix,
            int end_ix,
            Func<T, Point2D> getPoint)
        {
            if (Children.Count == 0)
                return new IndexFilter<T>(new T[0]);

            var r = Children[0].SimplifyIndexes(points, start_ix, end_ix, getPoint);

            for (int i = 1; i < Children.Count; i++)
            {
                r = Children[i].SimplifyIndexes(r, 0, r.Count - 1, getPoint);
            }

            return r;
        }
    }
}
