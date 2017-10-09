using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class IReadOnlyListExtensions
    {
        public static IReadOnlyList<T> EmptyIfNull<T>(this IReadOnlyList<T> items)
        {
            if (items == null)
                return new T[0];

            return items;
        }

        public static IList EmptyIfNull(this IList list)
        {
            if (list == null)
                return new List<object>();

            return list;
        }

        public static bool ElementsEqual<TItem>(
            this IReadOnlyList<TItem> x,
            IReadOnlyList<TItem> y)
        {
            return ElementsEqual<TItem>(x, y, ensureSequenceEqual: false, itemEqualityComparer: EqualityComparer<TItem>.Default);
        }

        public static bool ElementsEqual<TItem>(
            this IReadOnlyList<TItem> x,
            IReadOnlyList<TItem> y,
            bool ensureSequenceEqual)
        {
            return ElementsEqual<TItem>(x, y, ensureSequenceEqual, itemEqualityComparer: EqualityComparer<TItem>.Default);
        }

        public static bool ElementsEqual<TItem>(
            this IReadOnlyList<TItem> x,
            IReadOnlyList<TItem> y,
            bool ensureSequenceEqual,
            IEqualityComparer<TItem> itemEqualityComparer)
        {
            if (x == null || y == null)
                return true;

            if (x.Count != y.Count)
                return false;

            if (ensureSequenceEqual)
            {
                for (int i = 0; i < x.Count; i++)
                {
                    var xItem = x[i];
                    var yItem = y[i];

                    if (!itemEqualityComparer.Equals(xItem, yItem))
                        return false;
                }
            }

            var areAllEqual = x.Union(y, itemEqualityComparer).Count() == x.Count;

            return areAllEqual;
        }
    }
}
