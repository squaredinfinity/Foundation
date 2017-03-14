using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, Func<TSource> getDefaultValue)
        {
            if (source == null)
                throw new ArgumentException("source");
            else
                return DefaultIfEmptyIterator<TSource>(source, getDefaultValue);
        }

        static IEnumerable<TSource> DefaultIfEmptyIterator<TSource>(IEnumerable<TSource> source, Func<TSource> getDefaultValue)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    do
                    {
                        yield return enumerator.Current;
                    }
                    while (enumerator.MoveNext());
                }
                else
                    yield return getDefaultValue();
            }
        }

        public static IEnumerable<IEnumerable<T>> Chunkify<T>(
           this IEnumerable<T> enumerable,
           int chunkSize)
        {
            if (chunkSize < 1)
                throw new ArgumentException("chunkSize");

            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return enumerator.GetChunk(chunkSize);
                }
            }
        }

        private static IEnumerable<T> GetChunk<T>(
            this IEnumerator<T> enumerator,
            int chunkSize)
        {
            do
            {
                yield return enumerator.Current;
            }
            while (--chunkSize > 0 && enumerator.MoveNext());
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            if (list == null)
                return true;

            return !list.Any();
        }

        /// <summary>
        /// Returns *null* if list is null or empty, otherwise returns the input list.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToNullIfEmpty<T>(this IEnumerable<T> list)
        {
            if (list == null)
                return null;

            if (!list.Any())
                return null;

            return list;
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items)
        {
            if (items == null)
                return new EmptyEnumerator<T>();

            return items;
        }

        public static IEnumerable EmptyIfNull(this IEnumerable items)
        {
            if (items == null)
                return new EmptyEnumerator<object>();

            return items;
        }


        // todo: move to array extensions
        public static T[] EmptyIfNull<T>(this T[] list)
        {
            if (list == null)
                return new T[0];

            return list;
        }

        class EmptyEnumerator<T> : IEnumerator<T>, IEnumerable<T>, IEnumerator, IEnumerable
        {
            public object Current
            {
                get { return null; }
            }

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {
            }

            public IEnumerator GetEnumerator()
            {
                return this;
            }

            T IEnumerator<T>.Current
            {
                get { return default(T); }
            }

            public void Dispose()
            {
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this;
            }
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> list, T excludedElement)
        {
            return list.Except(new T[] { excludedElement });
        }

        public static int FastCount<T>(this IEnumerable<T> list)
        {
            var collection = list as ICollection;

            if(collection != null)
            {
                return collection.Count;
            }

            return list.Count();
        }

        /// <summary>
        /// Creates new List from enumerable.
        /// </summary>
        /// <param name="source">enumerable to create a List from</param>
        /// <param name="capacity">initial capacity of the list</param>
        /// <returns>List that contains elements from source.</returns>
        public static List<T> ToList<T>(this IEnumerable<T> source, int capacity)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            else
            {
                var list = new List<T>(capacity);
                
                list.AddRange(source);

                return list;
            }
        }

        public static ConcurrentDictionary<TKey, TSource> ToConcurrentDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return IEnumerableExtensions.ToConcurrentDictionary<TSource, TKey, TSource>(source, keySelector, IdentityFunction<TSource>.Instance, (IEqualityComparer<TKey>)null);
        }

        public static ConcurrentDictionary<TKey, TSource> ToConcurrentDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return IEnumerableExtensions.ToConcurrentDictionary<TSource, TKey, TSource>(source, keySelector, IdentityFunction<TSource>.Instance, comparer);
        }

        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return IEnumerableExtensions.ToConcurrentDictionary<TSource, TKey, TElement>(source, keySelector, elementSelector, (IEqualityComparer<TKey>)null);
        }

        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");
            if (elementSelector == null)
                throw new ArgumentNullException("elementSelector");

            comparer = comparer ?? EqualityComparer<TKey>.Default;

            ConcurrentDictionary<TKey, TElement> dictionary = new ConcurrentDictionary<TKey, TElement>(comparer);
            
            foreach (TSource source1 in source)
                dictionary.TryAdd(keySelector(source1), elementSelector(source1));

            return dictionary;
        }

        internal class IdentityFunction<TElement>
        {
            public static Func<TElement, TElement> Instance
            {
                get
                {
                    return (Func<TElement, TElement>)(x => x);
                }
            }

            public IdentityFunction()
            {
            }
        }
    }
}
