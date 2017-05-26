using System;
using System.Collections.Generic;

namespace SquaredInfinity.Comparers
{
    public class ComparerEx<T> : IComparerEx, IComparerEx<T>
    {
        public bool Compare(IComparable a, IComparable b, ComparisonType comparison)
        {
            var r = _Comparer.Default.Compare(a, b);

            switch (comparison)
            {
                case ComparisonType.LessThan:
                    return r < 0;
                case ComparisonType.LassOrEqual:
                    return r <= 0;
                case ComparisonType.Equal:
                    return r == 0;
                case ComparisonType.GreaterOrEqual:
                    return r >= 0;
                case ComparisonType.GreaterThan:
                    return r > 0;
                default:
                    throw new NotSupportedException($"specified comparison is not supported: {comparison.ToString()}");
            }
        }

        public bool Compare(T a, T b, ComparisonType comparison)
        {
            return Compare(a, b, Comparer<T>.Default, comparison);
        }

        public bool Compare(T a, T b, IComparer<T> comparer, ComparisonType comparison)
        {
            var r = comparer.Compare(a, b);

            switch (comparison)
            {
                case ComparisonType.LessThan:
                    return r < 0;
                case ComparisonType.LassOrEqual:
                    return r <= 0;
                case ComparisonType.Equal:
                    return r == 0;
                case ComparisonType.GreaterOrEqual:
                    return r >= 0;
                case ComparisonType.GreaterThan:
                    return r > 0;
                default:
                    throw new NotSupportedException($"specified comparison is not supported: {comparison.ToString()}");
            }
        }
    }
}
