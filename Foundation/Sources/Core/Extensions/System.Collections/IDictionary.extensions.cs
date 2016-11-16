using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class IDictionaryExtensions
    {
        public static void IfContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Action<TValue> action)
        {
            if (dict.ContainsKey(key))
                action(dict[key]);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> getDefaultValue)
        {
            if (dict.ContainsKey(key))
                return dict[key];

            return getDefaultValue();
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
        }

        public static void AddOrUpdateFrom<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> source)
        {
            foreach (var kvp in source)
            {
                dict[kvp.Key] = kvp.Value;
            }
        }

        public static void AddOrUpdateFrom<TKey, TValue>(this IDictionary<TKey, TValue> dict, IReadOnlyDictionary<TKey, TValue> source)
        {
            foreach(var kvp in source)
            {
                dict[kvp.Key] = kvp.Value;
            }
        }

        public static TValue EnsureKeyValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> createValue)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, createValue(key));

            return dict[key];
        }
    }
}
