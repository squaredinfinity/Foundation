using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation
{
    public static class SharedResourceDictionaryRepository
    {
        static readonly ConcurrentDictionary<string, SharedResourceDictionary> NameToDictionaryMappings =
            new ConcurrentDictionary<string, SharedResourceDictionary>();

        public static void AddOrUpdateSharedDictionary(SharedResourceDictionary dict)
        {
            NameToDictionaryMappings.AddOrUpdate(dict.UniqueName, dict, (_key, _old) => dict);
        }

        public static bool TryGetSharedDictionary(string uniqueName, out SharedResourceDictionary dict)
        {
            return NameToDictionaryMappings.TryGetValue(uniqueName, out dict);
        }
    }
}
