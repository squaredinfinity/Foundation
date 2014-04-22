using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ICollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static void AddIfNotNull<T>(this ICollection<T> collection, T value)
        {
            if (value == null)
                return;

            collection.Add(value);
        }

        public static void TryAddRange<T>(this ICollection<T> collection, params T[] items)
        {
            if (items == null)
                return;

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                collection.Add(item);
            }
        }

        /// <summary>
        /// Returns Read-Only copy of current collection.
        /// Unline AsReadonly() this creates a copy of collection, not just a wrapper.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static ReadOnlyCollection<T> ToReadOnly<T>(this ICollection<T> collection)
        {
            return new ReadOnlyCollection<T>(collection.ToArray());
        }
    }
}
