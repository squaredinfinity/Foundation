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
    }
}
