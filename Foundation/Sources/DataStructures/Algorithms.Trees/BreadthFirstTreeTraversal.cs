using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.DataStructures.Algorithms.Trees
{
    public class BreadthFirstTreeTraversal<TNode> : DynamicTreeTraversal<TNode>
    {
        public BreadthFirstTreeTraversal(Func<TNode, IEnumerable<TNode>> getChildren)
            : base(getChildren)
        { }

        protected override IEnumerable<TNode> DoTraverse(IEnumerable<TNode> rootLevel)
        {
            Queue<TNode> workQueue = new Queue<TNode>();

            foreach (var n in rootLevel)
                workQueue.Enqueue(n);

            return ProcessQueue(workQueue, GetChildren);
        }

        [Pure]
        IEnumerable<TNode> ProcessQueue(Queue<TNode> workQueue, Func<TNode, IEnumerable<TNode>> getChildren)
        {
            while (workQueue.Count != 0)
            {
                var item = workQueue.Dequeue();

                yield return item;

                foreach (var child in getChildren(item))
                {
                    workQueue.Enqueue(child);
                }
            }

            yield break;
        }
    }
}
