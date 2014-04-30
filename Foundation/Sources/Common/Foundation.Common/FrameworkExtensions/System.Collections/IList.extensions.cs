using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class IListExtensions
    {
        public static IList EmptyIfNull(this IList list)
        {
            if (list == null)
                return new List<object>();

            return list;
        }

        /// <summary>
        /// Checks if specified item candidate is compatible with this list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="listItemCandidate"></param>
        /// <returns></returns>
        public static bool CanAcceptItem(this IList list, object listItemCandidate)
        {
            // find which list interfaces are implemented by list
            var listInterfaces =
                (from i in list.GetType().GetInterfaces()
                 where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)
                 select i);

            var listItemTypes =
                (from i in listInterfaces
                 select i.GetGenericArguments().Single()).ToArray();

            // if no item types found, list accepts everything
            if (listItemTypes.Length == 0)
                return true;

            if (listItemCandidate is IEnumerable)
            {
                foreach (var item in listItemCandidate as IEnumerable)
                {
                    var listItemCandidateType = item.GetType();

                    var areTypesCompatible =
                        (from t in listItemTypes
                         where t.IsAssignableFrom(listItemCandidateType)
                         select t).Any();

                    if (!areTypesCompatible)
                        return false;
                }

                return true;
            }
            else
            {
                var listItemCandidateType = listItemCandidate.GetType();

                var areTypesCompatible =
                        (from t in listItemTypes
                         where t.IsAssignableFrom(listItemCandidateType)
                         select t).Any();

                return areTypesCompatible;
            }
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
            // find which list interfaces are implemented by list
            var listInterfaces =
                (from i in list.GetType().GetInterfaces()
                 where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)
                 select i);

            var listItemTypes =
                (from i in listInterfaces
                 select i.GetGenericArguments().Single()).ToList();

            return listItemTypes;
        }

        public static bool AreEqual<TItem>(
            IList<TItem> x,
            IList<TItem> y)
        {
            return AreEqual<TItem>(x, y, ensureSameSequence: true, itemEqualityComparer: EqualityComparer<TItem>.Default);
        }

        public static bool AreEqual<TItem>(
            IList<TItem> x,
            IList<TItem> y,
            bool ensureSameSequence,
            IEqualityComparer<TItem> itemEqualityComparer)
        {
            if (x == null || y == null)
                return false;

            if (x.Count != y.Count)
                return false;

            if (ensureSameSequence)
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
