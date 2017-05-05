using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.DataStructures.Algorithms.Trees
{
    /// <summary>
    /// Implementation of Tree Traversal algorithm
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public interface ITreeTraversal<TNode>
    {
        /// <summary>
        /// Traverse the tree starting at specified node
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        IEnumerable<TNode> Traverse(TNode root);

        /// <summary>
        /// Traverse the starting at specified nodes
        /// </summary>
        /// <param name="rootLevel"></param>
        /// <returns></returns>
        IEnumerable<TNode> Traverse(IEnumerable<TNode> rootLevel);
    }    
}
