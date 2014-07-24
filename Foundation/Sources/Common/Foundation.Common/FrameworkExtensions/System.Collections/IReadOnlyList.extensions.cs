using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class IReadOnlyListExtensions
    {
        public static IList EmptyIfNull(this IList list)
        {
            if (list == null)
                return new List<object>();

            return list;
        }

        /// <summary>
        /// Returns minimum base types of items accepted by the list.
        /// Note tha IList class may actually implement several generic IList interfaces and accept several different item types
        /// (e.g. by doing conversions internally)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="listItemCandidate"></param>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetItemsTypes(this IList list)
        {
            var listItemTypes = list.GetType().GetCompatibleItemTypes();

            return listItemTypes;
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
