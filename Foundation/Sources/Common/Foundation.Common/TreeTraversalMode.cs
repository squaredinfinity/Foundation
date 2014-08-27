using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    /// <summary>
    /// Tree traversal mode (Breadth First, Depth First, Bottom Up)
    /// </summary>
    public enum TreeTraversalMode
    {
        /// <summary>
        /// Start at root and explore all neightboring nodes, 
        /// then explore each neightbor node in turn folowing same rule (neightboring nodes first)
        /// </summary>
        BreadthFirst = 0,
        /// <summary>
        /// Start at root and explore as far as possible along each branch before backgracking.
        /// </summary>
        DepthFirst,
        /// <summary>
        /// Start at the leaf nodes and explore by moving up the tree
        /// </summary>
        BottomUp,
        /// <summary>
        /// Breadth First is the Default
        /// </summary>
        Default = BreadthFirst
    }
}
