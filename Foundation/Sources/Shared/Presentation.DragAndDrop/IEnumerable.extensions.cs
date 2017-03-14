using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SquaredInfinity.Presentation.DragDrop
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

        public static IEnumerable<TItem> ToArrayIfDebug<TItem>(this IEnumerable<TItem> list)
        {
            if (!Debugger.IsAttached)
                return list;

            return list.ToArray();
        }
    }
}
