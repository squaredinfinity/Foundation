using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.DataStructures.Algorithms.Trees
{
    public abstract class DynamicTreeTraversal<TNode> : ITreeTraversal<TNode>
    {
        protected Func<TNode, IEnumerable<TNode>> GetChildren { get; private set; }

        public DynamicTreeTraversal(Func<TNode, IEnumerable<TNode>> getChildren)
        {
            GetChildren = getChildren;
        }

        public IEnumerable<TNode> Traverse(TNode root)
        {
            return DoTraverse(root);
        }

        public IEnumerable<TNode> Traverse(IEnumerable<TNode> rootLevel)
        {
            return DoTraverse(rootLevel);
        }

        protected abstract IEnumerable<TNode> DoTraverse(IEnumerable<TNode> rootLevel);

        protected virtual IEnumerable<TNode> DoTraverse(TNode root)
        {
            return DoTraverse(new[] { root });
        }
    }
}
