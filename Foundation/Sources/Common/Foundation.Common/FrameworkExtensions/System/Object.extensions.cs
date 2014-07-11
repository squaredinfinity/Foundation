using SquaredInfinity.Foundation.Types.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ObjectExtensions
    {
        public static object GetValueOrDefault<T>(this T obj, T defaultValue)
        {
            if (obj == null)
                return defaultValue;

            return obj;
        }

        public static object GetValueOrDefault<T>(this T obj, Func<T> defaultValue)
        {
            if (obj == null)
                return defaultValue();

            return obj;
        }

        public static string ToString(this object obj, string nullValue)
        {
            if (obj == null)
                return nullValue;

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

        static readonly TypeMapper DefaultTypeMapper = new TypeMapper();

        public static void DeepCopyFrom<TTarget>(this TTarget target, object source)
        {
            target.DeepCopyFrom<TTarget>(source, MappingOptions.Default);
        }

        public static void DeepCopyFrom<TTarget>(this TTarget target, object source, MappingOptions options)
        {
            DefaultTypeMapper.Map(source, target, options);
        }

        public static TTarget DeepClone<TTarget>(this TTarget source)
        {
            return DefaultTypeMapper.DeepClone<TTarget>(source);
        }

        /// <summary>
        /// Create a weak event handler (subscription) for an event on source object.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static WeakEventHandlerPrototype<TSource> CreateWeakEventHandler<TSource>(this TSource source)
        {
            return new WeakEventHandlerPrototype<TSource>(source);
        }

        public static bool TryConvert(this object obj, Type targetType, out object result)
        {
            result = null;

            if (obj != null)
            {
                var objType = obj.GetType();

                if (objType.IsTypeEquivalentTo(targetType, treatNullableAsEquivalent: true, treatBaseTypesAsEquivalent: true, treatImplementedInterfacesAsEquivalent: true))
                {
                    result = obj;
                    return true;
                }
            }

            if(targetType.IsEnum)
            {
                if (obj is string)
                {
                    result = Enum.Parse(targetType, obj as string);
                }
                else
                {
                    result = Enum.ToObject(targetType, obj);
                }

                return true;
            }

            // todo: test if conversion enum -> string, enum -> int etc are supported

            if (obj is IConvertible)
            {
                result = System.Convert.ChangeType(obj, targetType);
                return true;
            }

            var converter = TypeDescriptor.GetConverter(obj);
            if(converter != null && converter.CanConvertTo(targetType))
            {
                result = converter.ConvertTo(obj, targetType);
                return true;
            }

            return false;
        }

        public static object Convert(this object obj, Type targetType)
        {
            var result = (object)null;

            if (TryConvert(obj, targetType, out result))
                return result;

            throw new InvalidOperationException("Unable to convert object to type {0}".FormatWith(targetType.FullName));
        }

        public static TTarget Convert<TTarget>(this object obj)
        {
            return (TTarget)Convert(obj, typeof(TTarget));
        }
    }
}
