using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Comparers
{
    public interface IComparerEx<T>
    {
        bool Compare(T a, T b, ComparisonType comparison);
        bool Compare(T a, T b, IComparer<T> comparer, ComparisonType comparison);
    }
}
