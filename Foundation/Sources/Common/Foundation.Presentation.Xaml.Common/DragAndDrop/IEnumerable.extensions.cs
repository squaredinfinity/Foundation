using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.DragDrop
{
    public static class IEnumerableExtensions
    {
        public static IList AsList(this IEnumerable list)
        {
            if (list is ICollectionView)
            {
                return ((ICollectionView)list).SourceCollection as IList;
            }
            else
            {
                return list as IList;
            }
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
            if(listItemTypes.Length == 0)
                return true;

            if(listItemCandidate is IEnumerable)
            {
                foreach(var item in listItemCandidate as IEnumerable)
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
    }
}
