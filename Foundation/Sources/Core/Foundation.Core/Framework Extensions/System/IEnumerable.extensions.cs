﻿using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public static class IEnumerableExtensions
    {
        public static MultiMap<TKey, TItem> ToMultiMap<TSource, TKey, TItem>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TItem> itemSelector)
        {
            var mm = new MultiMap<TKey, TItem>();

            foreach (TSource item in source)
                mm.Add(keySelector(item), itemSelector(item));

            return mm;
        }
    }
}