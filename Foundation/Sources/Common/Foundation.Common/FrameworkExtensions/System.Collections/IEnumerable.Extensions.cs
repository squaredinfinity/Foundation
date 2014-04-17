﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class IListExtensions
    {
        public static IList EmptyIfNull(this IList list)
        {
            if (list == null)
                return new List<object>();

            return list;
        }
    }

    public static class IEnumerableExtensions
    {
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

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
        {
            if (list == null)
                return new EmptyEnumerator<T>();

            return list;
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

        public static IEnumerable<T> TreeTraversal<T>(this IEnumerable<T> list)
        {
            return list.TreeTraversal(TreeTraversalMode.Default);
        }

        public static IEnumerable<T> TreeTraversal<T>(this IEnumerable<T> list, TreeTraversalMode traversalMode)
        {
            return ((T)list).TreeTraversal(traversalMode, DefaultGetChildrenFunc);
        }

        public static IEnumerable<T> DefaultGetChildrenFunc<T>(T parent)
        {
            IEnumerable<T> parentAsIEnumerable = parent as IEnumerable<T>;
            if (parent == null)
                yield break;

            foreach (var child in parentAsIEnumerable)
                yield return child;
        }

        public static IEnumerable<T> TreeTraversal<T>(this T me, Func<T, IEnumerable<T>> getChildrenFunc)
        {
            return me.TreeTraversal(TreeTraversalMode.Default, getChildrenFunc);
        }
        
        public static IEnumerable<T> TreeTraversal<T>(this T me, TreeTraversalMode traversalMode, Func<T, IEnumerable<T>> getChildrenFunc)
        {
            IEnumerable<T> result = null;

            switch (traversalMode)
            {
                case TreeTraversalMode.BreadthFirst:
                    result = me.BreadthFirstTreeTraversal(getChildrenFunc);
                    break;
                case TreeTraversalMode.DepthFirst:
                    result = me.DepthFirstTreeTraversal(getChildrenFunc);
                    break;
                case TreeTraversalMode.BottomUp:
                    result = me.BottomUpTreeTraversal(getChildrenFunc);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Uses Depth-First algorithm to traverse the tree.
        /// http://en.wikipedia.org/wiki/Depth-first_search
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        static IEnumerable<T> DepthFirstTreeTraversal<T>(this T me, Func<T, IEnumerable<T>> getChildrenFunc)
        {
            Stack<T> workQueue = new Stack<T>();

            workQueue.Push(me);

            while (workQueue.Count != 0)
            {
                var item = workQueue.Pop();

                if (item == null)
                    continue;

                foreach (var child in getChildrenFunc(item).Reverse())
                {
                    workQueue.Push(child);
                }

                yield return item;
            }

            yield break;
        }

        /// <summary>
        /// Uses Breadth-First algorithm to traverse the tree.
        /// http://en.wikipedia.org/wiki/Breadth-first_traversal
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        static IEnumerable<T> BreadthFirstTreeTraversal<T>(this T me, Func<T, IEnumerable<T>> getChildrenFunc)
        {
            Queue<T> workQueue = new Queue<T>();

            workQueue.Enqueue(me);

            while (workQueue.Count != 0)
            {
                var item = workQueue.Dequeue();

                if (item == null)
                    continue;

                foreach (var child in getChildrenFunc(item))
                {
                    workQueue.Enqueue(child);
                }

                yield return item;
            }

            yield break;
        }

        /// <summary>
        /// Returns nodes starting from leafes and working its way up in layers.
        /// a node of depth n won't be returned unless all nodes with depth n+1 have been returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="me"></param>
        /// <param name="getChildrenFunc"></param>
        /// <returns></returns>
        static IEnumerable<T> BottomUpTreeTraversal<T>(this T me, Func<T, IEnumerable<T>> getChildrenFunc)
        {
            Stack<T> results = new Stack<T>();
            
            Queue<T> workQueue = new Queue<T>();

            workQueue.Enqueue(me);

            while (workQueue.Count != 0)
            {
                var item = workQueue.Dequeue();

                if (item == null)
                    continue;

                results.Push(item);

                foreach (var child in getChildrenFunc(item).Reverse())
                {
                    workQueue.Enqueue(child);
                }
            }

            while (results.Count > 0)
                yield return results.Pop();
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
