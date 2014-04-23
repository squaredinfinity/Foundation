using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class IDictionaryExtensions
    {
        // todo: this is now available in .net4.5, quite sure wasn't there before   
        //public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, out TValue value)
        //{
        //    if(dict != null && dict.ContainsKey(key))
        //    {
        //        value = dict[key];
        //        return true;
        //    }

        //    value = default(TValue);
        //    return false;
        //}


        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
        }

        public static void AddOrUpdateFrom<TKey, TValue>(this IDictionary<TKey, TValue> dict, IReadOnlyDictionary<TKey, TValue> source)
        {
            foreach(var kvp in source)
            {
                dict[kvp.Key] = kvp.Value;
            }
        }
    }
}
