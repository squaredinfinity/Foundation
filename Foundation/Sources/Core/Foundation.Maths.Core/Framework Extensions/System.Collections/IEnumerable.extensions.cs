using SquaredInfinity.Foundation.Maths.Graphs.Trees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class IEnumerableExtensions
    {
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
            // NOT SUPPORTED IN CORE:
            //if (list.GetType().IsTypeEquivalentTo(typeof(TTreeNode)) || list.GetType().ImplementsOrExtends(typeof(TTreeNode)))
            //{
            //    return ((TTreeNode)list).TreeTraversal(traversalMode, DefaultGetChildrenFunc);
            //}
            //else
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

            foreach (var item in initialItems)
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

            foreach (var item in initialItems)
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

            foreach (var item in initialItems)
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
    }
}
