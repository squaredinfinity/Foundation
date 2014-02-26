using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public static class TypeExtensions
    {
        // TODO: documentation for this
        // seem to resolve assemblies while ignoring asm name case and version (?)
        public static readonly Func<AssemblyName, Assembly> LastHopeAssemblyResolver = (an) => ResolveAssembly(an);

        static Assembly ResolveAssembly(AssemblyName name)
        {
            var result =
                (from asm in AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName)
                 where asm.FullName.StartsWith(name.FullName)
                 select asm).FirstOrDefault();

            return result;
        }


        public static readonly Func<Assembly, string, bool, Type> LastHopeTypeResolver = (asm, typeName, ignoreCase) => ResolveType(asm, typeName, ignoreCase);

        static Type ResolveType(Assembly assemly, string typeName, bool ignoreCase)
        {
            if (assemly != null)
                return assemly.GetType(typeName, throwOnError: false, ignoreCase: ignoreCase);

            var result =
                (from asm in AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName)
                 let type = asm.GetType(typeName, throwOnError: false, ignoreCase: ignoreCase)
                 where type != null
                 select type).FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Returns true if given type implements specified interface.
        /// </summary>
        /// <typeparam name="TInterfaceType"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ImplementsInterface<TInterfaceType>(this Type type)
        {
            return type.GetInterface(typeof(TInterfaceType).FullName, true) != null;
        }

        /// <summary>
        /// Returns true if given type implements specified interface.
        /// </summary>
        /// <typeparam name="TInterfaceType"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ImplementsInterface(this Type type, string interfaceFullName)
        {
            return type.GetInterface(interfaceFullName, true) != null;
        }

        /// <summary>
        /// Checks if type is equivalent to other type.
        /// By default, returns true only when specified types are exactly the same.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="otherType">other type</param>
        /// <param name="allowNullable">When set to true, Nullable T (where T is the same type as *type* parameter) will be treated as equivalent.</param>
        /// <returns></returns>
        public static bool IsTypeEquivalentTo(this Type type, Type otherType, bool treatNullableAsEquivalent = false)
        {
            if (otherType == null)
                throw new ArgumentNullException("otherType");

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
    }
}
