using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns true if given type implements specified interface.
        /// </summary>
        /// <typeparam name="TInterfaceType"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ImplementsInterface<TInterfaceType>(this Type type)
        {
            return type.ImplementsInterface(typeof(TInterfaceType));
        }

        /// <summary>
        /// Returns true if given type implements specified interface.
        /// </summary>
        /// <typeparam name="TInterfaceType"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            var result =
                (from i in type.GetInterfaces()
                 where string.Equals(i.AssemblyQualifiedName, interfaceType.AssemblyQualifiedName, StringComparison.InvariantCultureIgnoreCase)
                 select i).Any();

            return result;
        }

        /// <summary>
        /// Returns true if given type implements specified interface.
        /// </summary>
        /// <typeparam name="TInterfaceType"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ImplementsInterface(this Type type, string interfaceFullName)
        {
            var result =
                (from i in type.GetInterfaces()
                 where string.Equals(i.FullName, interfaceFullName, StringComparison.InvariantCultureIgnoreCase)
                 select i).Any();

            return result;
        }
        
        /// <summary>
        /// Checks if type is equivalent to other type.
        /// By default, returns true only when specified types are exactly the same.
        /// By default implemented interfaces and base types are *NOT* considered equivalent.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="otherType">other type</param>
        /// <param name="allowNullable">When set to true, Nullable T (where T is the same type as *type* parameter) will be treated as equivalent.</param>
        /// <returns></returns>
        public static bool IsTypeEquivalentTo(
            this Type type,
            Type otherType, 
            bool treatNullableAsEquivalent = false,
            bool treatBaseTypesAsEquivalent = false,
            bool treatImplementedInterfacesAsEquivalent = false)
        {
            if (otherType == null)
                throw new ArgumentNullException("otherType");

            if (treatBaseTypesAsEquivalent && type.IsAssignableFrom(otherType))
                return true;

            if (treatImplementedInterfacesAsEquivalent && type.ImplementsInterface(otherType))
                return true;

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

        public static bool IsNullable(this Type type)
        {
            if (Nullable.GetUnderlyingType(type) == null)
                return false;

            return true;
        }
    }
}
