using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class IListExtensions
    {
        /// <summary>
        /// Checks if specified item candidate is compatible with this list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="listItemCandidate"></param>
        /// <returns></returns>
        public static bool CanAcceptItem(this IList list, object listItemCandidate, IReadOnlyList<Type> compatibleItemTypes = null)
        {
            var listType = list.GetType();

            if(compatibleItemTypes == null)
                compatibleItemTypes = listType.GetCompatibleItemTypes();

            // if no item types found, list accepts everything
            if (compatibleItemTypes.Count == 0)
                return true;

            if (listItemCandidate is IEnumerable)
            {
                foreach (var item in listItemCandidate as IEnumerable)
                {
                    var listItemCandidateType = item.GetType();

                    var areTypesCompatible =
                        (from t in compatibleItemTypes
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

                return listType.CanAcceptItem(listItemCandidateType, compatibleItemTypes);
            }
        }
    }
}
