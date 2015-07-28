using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns minimum base types of items accepted by the enumerable.
        /// Note that Enumerable class may actually implement several generic IEnumerable interfaces and accept several different item types
        /// (e.g. by doing conversions internally)
        /// </summary>
        /// <param name="list"></param>
        /// <param name="listItemCandidate"></param>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetCompatibleItemsTypes(this IEnumerable source)
        {
            var listItemTypes = source.GetType().GetCompatibleItemTypes();

            return listItemTypes;
        }

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

        public static IEnumerable<object> OfType(
            this IEnumerable source, 
            Type type,
            bool treatNullableAsEquivalent = false)
        {
            foreach (object obj in source)
            {
                if (obj == null)
                    continue;

                if (obj.GetType().IsTypeEquivalentTo(type, treatNullableAsEquivalent))
                    yield return obj;
                else if (obj.GetType().ImplementsOrExtends(type))
                    yield return obj;
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

        public static IEnumerable<T> TreeTraversal<T>(this IEnumerable<T> list)
        {
            return list.TreeTraversal(TreeTraversalMode.Default);
        }

        public static IEnumerable<TTreeNode> TreeTraversal<TTreeNode>(
            this IEnumerable<TTreeNode> list, 
            TreeTraversalMode traversalMode)
        {
            //  if list is of T type itself (e.g. it's a node in a tree hierarchy)
            //  then process the list itself
            if (list.GetType().IsTypeEquivalentTo(typeof(TTreeNode)) || list.GetType().ImplementsOrExtends(typeof(TTreeNode)))
            {
                return ((TTreeNode)list).TreeTraversal(traversalMode, DefaultGetChildrenFunc);
            }
            else
            {
                switch (traversalMode)
                {
                    case TreeTraversalMode.BreadthFirst:
                        return BreadthFirstTreeTraversalInternal(list, DefaultGetChildrenFunc);
                    case TreeTraversalMode.DepthFirst:
                        return DepthFirstTreeTraversalInternal(list, DefaultGetChildrenFunc);
                    case TreeTraversalMode.BottomUp:
                        return BottomUpTreeTraversalInternal(list, DefaultGetChildrenFunc);
                }

                return null;
            }
        }

        public static IEnumerable<TTreeNode> DefaultGetChildrenFunc<TTreeNode>(TTreeNode parent)
        {
            IEnumerable<TTreeNode> parentAsIEnumerable = parent as IEnumerable<TTreeNode>;
            if (parentAsIEnumerable == null)
                yield break;

            foreach (var child in parentAsIEnumerable)
                yield return child;
        }

        public static IEnumerable<TTreeNode> TreeTraversal<TTreeNode>(
            this TTreeNode root, 
            Func<TTreeNode, 
            IEnumerable<TTreeNode>> getChildrenFunc)
        {
            return root.TreeTraversal(TreeTraversalMode.Default, getChildrenFunc);
        }

        public static IEnumerable<TTreeNode> TreeTraversal<TTreeNode>(
            this TTreeNode root,
            TreeTraversalMode traversalMode,
            Func<TTreeNode, IEnumerable<TTreeNode>> getChildrenFunc)
        {
            IEnumerable<TTreeNode> result = null;

            switch (traversalMode)
            {
                case TreeTraversalMode.BreadthFirst:
                    result = root.BreadthFirstTreeTraversal(getChildrenFunc);
                    break;
                case TreeTraversalMode.DepthFirst:
                    result = root.DepthFirstTreeTraversal(getChildrenFunc);
                    break;
                case TreeTraversalMode.BottomUp:
                    result = root.BottomUpTreeTraversal(getChildrenFunc);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Uses Depth-First algorithm to traverse the tree.
        /// http://en.wikipedia.org/wiki/Depth-first_search
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        static IEnumerable<TTreeNode> DepthFirstTreeTraversal<TTreeNode>(
            this TTreeNode root,
            Func<TTreeNode,
            IEnumerable<TTreeNode>> getChildrenFunc)
        {
            Stack<TTreeNode> workQueue = new Stack<TTreeNode>();

            workQueue.Push(root);

            return DepthFirstTreeTraversalInternal<TTreeNode>(workQueue, getChildrenFunc);
        }

        static IEnumerable<TTreeNode> DepthFirstTreeTraversalInternal<TTreeNode>(
            IEnumerable<TTreeNode> initialItems,
            Func<TTreeNode, 
            IEnumerable<TTreeNode>> getChildrenFunc)
        {
            Stack<TTreeNode> workQueue = new Stack<TTreeNode>();

            foreach(var item in initialItems)
                workQueue.Push(item);

            return DepthFirstTreeTraversalInternal<TTreeNode>(workQueue, getChildrenFunc);
        }

        static IEnumerable<T> DepthFirstTreeTraversalInternal<T>(Stack<T> workQueue, Func<T, IEnumerable<T>> getChildrenFunc)
        {
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

            return BreadthFirstTreeTraversalInternal<T>(workQueue, getChildrenFunc);
        }

        static IEnumerable<T> BreadthFirstTreeTraversalInternal<T>(IEnumerable<T> initialItems, Func<T, IEnumerable<T>> getChildrenFunc)
        {
            Queue<T> workQueue = new Queue<T>();

            foreach(var item in initialItems)
                workQueue.Enqueue(item);

            return BreadthFirstTreeTraversalInternal<T>(workQueue, getChildrenFunc);
        }

        static IEnumerable<T> BreadthFirstTreeTraversalInternal<T>(Queue<T> workQueue, Func<T, IEnumerable<T>> getChildrenFunc)
        {
            while (workQueue.Count != 0)
            {
                var item = workQueue.Dequeue();

                if (item == null)
                    continue;

                foreach (var child in getChildrenFunc(item))
                {
                    workQueue.Enqueue(child);
                }

                // todo: should this yield be before getting children?
                // that way children would not be enqueued if iteration stopped by caller
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
            Queue<T> workQueue = new Queue<T>();

            workQueue.Enqueue(me);

            return BottomUpTreeTraversalInternal<T>(workQueue, getChildrenFunc);
        }

        static IEnumerable<T> BottomUpTreeTraversalInternal<T>(IEnumerable<T> initialItems, Func<T, IEnumerable<T>> getChildrenFunc)
        {
            Queue<T> workQueue = new Queue<T>();

            foreach(var item in initialItems)
                workQueue.Enqueue(item);

            return BottomUpTreeTraversalInternal<T>(workQueue, getChildrenFunc);
        }

        static IEnumerable<T> BottomUpTreeTraversalInternal<T>(Queue<T> workQueue, Func<T, IEnumerable<T>> getChildrenFunc)
        {
            Stack<T> results = new Stack<T>();

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
