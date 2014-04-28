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


        public static readonly Func<Assembly, string, bool, Type> LastHopeTypeResolver =
            (asm, typeFullName, ignoreCase) => ResolveType(typeFullName, ignoreCase, new List<Assembly> { asm });

        public static Type ResolveType(
            string typeFullOrPartialName, 
            bool ignoreCase,
            IReadOnlyList<Assembly> assembliesToCheck = null,
            IReadOnlyList<Type> baseTypes = null)
        {
            var result = 
                ResolveTypeFromFullName(
                typeFullOrPartialName, 
                ignoreCase, 
                assembliesToCheck);

            if (result != null)
                return result;

            return 
                ResolveTypeFromPartialName(
                typeFullOrPartialName, 
                ignoreCase, 
                assembliesToCheck,
                baseTypes)
                .FirstOrDefault();
        }

        public static Type ResolveTypeFromFullName(
            string typeFullName, 
            bool ignoreCase,
            IReadOnlyList<Assembly> assembliesToCheck = null)
        {
            if (assembliesToCheck == null)
            {
                var asms = new List<Assembly>();
                asms.AddRange(AppDomain.CurrentDomain.GetAssemblies());

                assembliesToCheck = asms;
            }

            var result =
                (from asm in assembliesToCheck
                 let type = asm.GetType(typeFullName, throwOnError: false, ignoreCase: ignoreCase)
                 where type != null
                 select type).FirstOrDefault();

            return result;
        }

        public static IReadOnlyList<Type> ResolveTypeFromPartialName(
            string typePartialName, 
            bool ignoreCase,
            IReadOnlyList<Assembly> assembliesToCheck = null,
            IReadOnlyList<Type> baseTypes = null)
        {
            if (assembliesToCheck == null)
            {
                var asms = new List<Assembly>();
                asms.AddRange(AppDomain.CurrentDomain.GetAssemblies());

                assembliesToCheck = asms;
            }

            var results =
                (from asm in assembliesToCheck
                 from t in asm.GetTypes()
                 where t.Name.EndsWith(typePartialName, ignoreCase, CultureInfo.InvariantCulture)
                 select t).ToList();

            if(baseTypes != null)
            {
                for(int i = results.Count - 1; i >= 0; i--)
                {
                    var result_candidate = results[i];

                    foreach(var t in baseTypes)
                    {
                        if(t.IsInterface && !result_candidate.ImplementsInterface(t))
                        {
                            results.RemoveAt(i);
                        }
                        else if(!result_candidate.IsAssignableFrom(t))
                        {
                            results.RemoveAt(i);
                        }
                    }
                }
            }

            return results;
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
            return type.GetInterface(interfaceType.FullName, ignoreCase: true) != null;
        }

        /// <summary>
        /// Returns true if given type implements specified interface.
        /// </summary>
        /// <typeparam name="TInterfaceType"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ImplementsInterface(this Type type, string interfaceFullName)
        {
            return type.GetInterface(interfaceFullName, ignoreCase:true) != null;
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
