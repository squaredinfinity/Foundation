using SquaredInfinity.Collections;
using SquaredInfinity.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.DigitalSignalProcessing.PolygonSimplification
{
    public interface IPolygon2DIndexesSimplification
    {
        //  IMPLEMENTATION:
        //      Simplify*Indexes* retunr IIndexFilter
        //      this allows to combine multiple algorithms in an efficient way without having to copy filtred elements from one filtered list to another
        //      the copy of all filtered elements can still be obtained at the end of the process by calling .ToArray() or .ToList()
        //      This approach can only be used in algorithms that preserve original points (i.e. do not add new points)

        IIndexFilter<T> SimplifyIndexes<T>(
            IReadOnlyList<T> points,
            Func<T, Point2D> getPoint);

        IIndexFilter<T> SimplifyIndexes<T>(
            IReadOnlyList<T> points,
            int start_ix,
            int end_ix,
            Func<T, Point2D> getPoint);
    }
}
