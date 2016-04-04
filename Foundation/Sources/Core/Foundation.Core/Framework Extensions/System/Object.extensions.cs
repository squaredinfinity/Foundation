using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ObjectExtensions
    {
        public static T GetValueOrDefault<T>(this T obj, T defaultValue)
        {
            if (obj == null)
                return defaultValue;

            return obj;
        }

        public static T GetValueOrDefault<T>(this T obj, Func<T> defaultValue)
        {
            if (obj == null)
                return defaultValue();

            return obj;
        }

        public static string ToString(this object obj, string valueWhenNull)
        {
            if (obj == null)
                return valueWhenNull;

            return obj.ToString();
        }

        public static string ToString(this object obj, string valueWhenNull, string valueWhenEmpty)
        {
            if (obj == null)
                return valueWhenNull;

            var toString = obj.ToString();

            if (string.IsNullOrEmpty(toString))
                return valueWhenEmpty;

            return toString;
        }

        /// <summary>
        /// Determines whether the specified object (value) is in a list.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the specified me is in the list; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIn<T>(this T obj, params T[] list)
        {
            return list.Contains(obj);
        }

        /// <summary>
        /// Determines whether the specified object (value) is in a list.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the specified me is in the list; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIn<T>(this T sourceItem, IEnumerable<T> listToCheckAgainst)
        {
            return listToCheckAgainst.Contains(sourceItem);
        }

        /// <summary>
        /// Determines whether the specified object (value) is in a list.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the specified me is in the list; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIn<T>(this T obj, IEqualityComparer<T> equalityComparer, params T[] list)
        {
            return list.Contains(obj, equalityComparer);
        }

        /// <summary>
        /// Determines whether the specified object (value) is in a list.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if the specified me is in the list; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIn<T>(this T obj, IEqualityComparer<T> equalityComparer, IEnumerable<T> list)
        {
            return list.Contains(obj, equalityComparer);
        }
    }
}
