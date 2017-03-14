using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class ConcurrentBagExtensions
    {
        /// <summary>
        /// Removes all items from the bag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bag"></param>
        public static void EnsureIsEmpty<T>(this ConcurrentBag<T> bag)
        {
            T _ignore;

            while (bag.TryTake(out _ignore))
            { }
        }
    }
}
