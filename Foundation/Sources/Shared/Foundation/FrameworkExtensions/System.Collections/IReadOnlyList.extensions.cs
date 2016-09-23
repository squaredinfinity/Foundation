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
        /// <summary>
        /// Returns minimum base types of items accepted by the list.
        /// Note tha IList class may actually implement several generic IList interfaces and accept several different item types
        /// (e.g. by doing conversions internally)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="listItemCandidate"></param>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetCompatibleItemsTypes(this IList list)
        {
            var listItemTypes = list.GetType().GetCompatibleItemTypes();

            return listItemTypes;
        }
    }
}
