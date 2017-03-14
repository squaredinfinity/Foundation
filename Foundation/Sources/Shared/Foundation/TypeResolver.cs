using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using System.Collections.Concurrent;

namespace SquaredInfinity
{
    public class TypeResolver
    {
        public static TypeResolver Default { get; private set; }

        static TypeResolver()
        {
            Default = new TypeResolver();
        }

        readonly ConcurrentDictionary<string, List<Type>> IgnoreCaseNameToTypeCache =
            new ConcurrentDictionary<string, List<Type>>();

        readonly ConcurrentDictionary<string, List<Type>> NameToTypeCache =
            new ConcurrentDictionary<string, List<Type>>();

        public readonly Func<AssemblyName, Assembly> LastHopeAssemblyResolver;
        public readonly Func<Assembly, string, bool, bool, Type> LastHopeTypeResolver;

        public TypeResolver()
        {
            LastHopeAssemblyResolver =  
                (an) => ResolveAssembly(an);

            LastHopeTypeResolver =
                (asm, typeFullName, ignoreCase, treatNullableAsEquivalent) => ResolveType(typeFullName, ignoreCase, treatNullableAsEquivalent, new List<Assembly> { asm });
        }

        public void ClearCache()
        {
            IgnoreCaseNameToTypeCache.Clear();
        }

        public virtual Assembly ResolveAssembly(AssemblyName name)
        {
            var result =
                (from asm in AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName)
                 where asm.FullName.StartsWith(name.FullName)
                 select asm).FirstOrDefault();

            return result;
        }

        public virtual Type ResolveType(
            string typeFullOrPartialName,
            bool ignoreCase,
            bool treatNullableAsEquivalent = true,
            IReadOnlyList<Assembly> assembliesToCheck = null,
            IReadOnlyList<Type> baseTypes = null)
        {
            if(baseTypes != null && baseTypes.Count == 1)
            {
                var baseType = baseTypes[0];

                if (baseType.FullName == typeFullOrPartialName)
                    return baseType;

                if (baseType.Name == typeFullOrPartialName)
                    return baseType;

                if (treatNullableAsEquivalent)
                {
                    var nullableUnderlyingType = Nullable.GetUnderlyingType(baseType);
                    
                    if (nullableUnderlyingType != null && nullableUnderlyingType.Name == typeFullOrPartialName)
                        return baseType;
                }

            }

            //if (baseTypes.IsNullOrEmpty())
            {
                ConcurrentDictionary<string, List<Type>> cache = IgnoreCaseNameToTypeCache;

                if (!ignoreCase)
                    cache = NameToTypeCache;

                var candidates =
                    cache.GetOrAdd(
                    typeFullOrPartialName,
                    (_) =>
                    {
                        var x = new List<Type>();

                        var t = ResolveTypeInternal(typeFullOrPartialName, ignoreCase, assembliesToCheck, baseTypes);

                        x.Add(t);

                        return x;
                    });

                if (baseTypes.IsNullOrEmpty())
                {
                    var result = candidates.FirstOrDefault();
                    if (result != null)
                        return result;
                }
                else
                {
                    foreach (var c in candidates)
                    {
                        foreach (var bt in baseTypes)
                        {
                            if (bt.IsAssignableFrom(c))
                                return c;
                        }
                    }
                }

                //if (candidates.Count == 1)
                    //return candidates.Single();

              //  return null;
            }
            //else
            {
                // todo: add cache which supports base types
                return ResolveTypeInternal(typeFullOrPartialName, ignoreCase, assembliesToCheck, baseTypes);
            }
        }

        Type ResolveTypeInternal(
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

        public virtual Type ResolveTypeFromFullName(
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

        public virtual IReadOnlyList<Type> ResolveTypeFromPartialName(
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

            var candidates =
                (from asm in assembliesToCheck
                 from t in asm.GetTypes()
                 where string.Equals(t.Name, typePartialName, StringComparison.InvariantCultureIgnoreCase)
                 select t);

            return ResolveTypes(candidates, baseTypes);
        }

        public static IReadOnlyList<Type> ResolveTypes(IEnumerable<Type> candidates, IReadOnlyList<Type> baseTypes)
        {
            var results = candidates.ToList();

            if (baseTypes != null)
            {
                for (int i = results.Count - 1; i >= 0; i--)
                {
                    var result_candidate = results[i];

                    foreach (var t in baseTypes)
                    {
                        if (t.IsInterface)
                        {
                            if (!result_candidate.ImplementsInterface(t))
                                results.RemoveAt(i);
                        }
                        else
                        {
                            if (!t.IsAssignableFrom(result_candidate))
                                results.RemoveAt(i);
                        }
                    }
                }
            }

            return results;
        }
    }
}
