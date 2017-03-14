using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class TypeExtensions
    {

        /// <summary>
        /// Returns true if types are equivalent.
        /// Optionaly can treat nullable version of a non-nullable type T as equivalent to T.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="otherType">other type</param>
        /// <param name="allowNullable">When set to true, Nullable T (where T is the same type as *type* parameter) will be treated as equivalent.</param>
        /// <returns></returns>
        public static bool IsTypeEquivalentTo(
            this Type type,
            Type otherType,
            bool treatNullableAsEquivalent = false,
            bool treatBaseTypesAndinterfacesAsEquivalent = false)
        {
            if (otherType == null)
                throw new ArgumentNullException("otherType");

            if (treatBaseTypesAndinterfacesAsEquivalent)
            {
                if (type.ImplementsOrExtends(otherType))
                    return true;
            }

            if (treatNullableAsEquivalent)
            {
                if (type == otherType)
                    return true;

                return type == Nullable.GetUnderlyingType(otherType);
            }
            else
            {
                return type == otherType;
            }
        }

        public static bool ImplementsOrExtends(this Type type, Type otherType)
        {
            var result = otherType.IsAssignableFrom(type);

            return result;
        }

        public static bool IsNullable(this Type type)
        {
            if (!type.IsValueType)
                return true;

            if (Nullable.GetUnderlyingType(type) == null)
                return false;

            return true;
        }
    }
}
