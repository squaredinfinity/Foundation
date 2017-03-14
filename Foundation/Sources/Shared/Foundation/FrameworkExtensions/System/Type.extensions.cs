﻿using System;
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
        /// When type implements IEnumerable,
        /// this will return the minimum list of compatible types supported as elements.
        /// (other types may still be supported if they implement or inherit from types in this list).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetCompatibleItemTypes(this Type type)
        {
            // find which list interfaces are implemented by IEnumerable
            var listInterfaces =
                (from i in type.GetInterfaces()
                 where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                 select i);

            var listItemTypes =
                (from i in listInterfaces
                 select i.GetGenericArguments().Single()).ToArray();

            return listItemTypes;
        }

        public static bool CanAcceptItem(this Type collectionType, Type itemCandidateType, IReadOnlyList<Type> compatibleItemTypes = null)
        {
            if (compatibleItemTypes == null)
                compatibleItemTypes = collectionType.GetCompatibleItemTypes();

            // if no item types found, collection accepts everything
            if (compatibleItemTypes.Count == 0)
                return true;

            var areTypesCompatible =
                    (from t in compatibleItemTypes
                     where t.IsAssignableFrom(itemCandidateType)
                     select t).Any();

            return areTypesCompatible;
        }

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
    }
}
