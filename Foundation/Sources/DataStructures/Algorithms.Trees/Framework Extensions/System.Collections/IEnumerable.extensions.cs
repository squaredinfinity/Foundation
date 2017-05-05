using SquaredInfinity.DataStructures.Algorithms.Trees;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SquaredInfinity.Extensions
{
    class EnumerableTreeTraversal<T> : BreadthFirstTreeTraversal<T>
    {
        public EnumerableTreeTraversal()
            : this(GetChildrenInternal)
        { }

        public EnumerableTreeTraversal(Func<T, IEnumerable<T>> getChildren) : base(getChildren)
        {
            
        }

        public static IEnumerable<T> GetChildrenInternal<T>(T root)
        {
            IEnumerable<T> _as_enumerable = root as IEnumerable<T>;

            if (_as_enumerable == null)
                yield break;

            foreach (var child in _as_enumerable)
                yield return child;
        }
    }

    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> TreeTraversal<T>(this IEnumerable<T> list)
        {
            var tt = new EnumerableTreeTraversal<T>();

            return tt.Traverse(list);
        }
    }
}
