using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns minimum base types of items accepted by the enumerable.
        /// Note that Enumerable class may actually implement several generic IEnumerable interfaces and accept several different item types
        /// (e.g. by doing conversions internally)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="listItemCandidate"></param>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetCompatibleItemsTypes(this IEnumerable source)
        {
            var listItemTypes = source.GetType().GetCompatibleItemTypes();

            return listItemTypes;
        }

        public static IEnumerable<object> OfType(
    this IEnumerable source,
    Type type,
    bool treatNullableAsEquivalent = false)
        {
            foreach (object obj in source)
            {
                if (obj == null)
                    continue;

                if (obj.GetType().IsTypeEquivalentTo(type, treatNullableAsEquivalent))
                    yield return obj;
                else if (obj.GetType().ImplementsOrExtends(type))
                    yield return obj;
            }
        }
    }
}
