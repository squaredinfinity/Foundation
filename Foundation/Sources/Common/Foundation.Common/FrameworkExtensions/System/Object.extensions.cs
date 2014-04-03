using SquaredInfinity.Foundation.Types.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ObjectExtensions
    {
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

        static readonly TypeMapper DefaultTypeMapper = new TypeMapper();

        public static void DeepCopyFrom<TTarget>(this TTarget target, object source)
            where TTarget : class, new()
        {
            target.DeepCopyFrom<TTarget>(source, MappingOptions.Default);
        }

        public static void DeepCopyFrom<TTarget>(this TTarget target, object source, MappingOptions options)
            where TTarget : class, new()
        {
            DefaultTypeMapper.Map(source, target, options);
        }

        public static TTarget DeepClone<TTarget>(this TTarget source)
            where TTarget : class, new()
        {
            return DefaultTypeMapper.DeepClone<TTarget>(source);
        }
    }
}
