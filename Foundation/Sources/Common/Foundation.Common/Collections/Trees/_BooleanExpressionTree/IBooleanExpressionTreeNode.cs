using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections.Trees
{
    public interface IBooleanExpressionTreeNode
    {
        event EventHandler<EventArgs> AfterTreeChanged;

        IBooleanExpressionTreeNode Parent { get; }
        IBooleanExpressionTreeNode Left { get; }
        IBooleanExpressionTreeNode Right { get; }

        bool Evaluate(object payload);

        /// <summary>
        /// Node with lower precedence is evaluated first
        /// </summary>
        int GetPrecedence();

        IEnumerable<IBooleanExpressionTreeNode> TraverseTreeInOrder();
        IEnumerable<IBooleanExpressionTreeNode> TraverseTreePostOrder();

        /// <summary>
        /// Inserts specified node as a Left child of this node.
        /// Steps will be taken to ensure tree correctness after this operation.
        /// </summary>
        /// <param name="leftNode"></param>
        void InsertLeft(IBooleanExpressionTreeNode leftNode);

        /// <summary>
        /// Inserts specified node as a Right child of this node.
        /// Steps will be taken to ensure tree correctness after this operation.
        /// </summary>
        /// <param name="rightNode"></param>
        void InsertRight(IBooleanExpressionTreeNode rightNode);

        /// <summary>
        /// Injects this node into tree hierarchy of specified node.
        /// </summary>
        /// <param name="node"></param>
        IBooleanExpressionTreeNode InjectInto(IBooleanExpressionTreeNode node);

        /// <summary>
        /// Replaces old child node with a new child node.
        /// Steps will be taken to ensure tree correctness after this operation.
        /// Old node will be completely removed from the tree.
        /// </summary>
        /// <param name="oldNode"></param>
        /// <param name="newNode"></param>
        void ReplaceChildNode(IBooleanExpressionTreeNode oldNode, IBooleanExpressionTreeNode newNode);

        void SwapChildren();

        /// <summary>
        /// Assigns a new Parent (or null) to this Node.
        /// No further action will be taken to ensure tree correctness.
        /// </summary>
        /// <param name="newParent"></param>
        void AssignParent(IBooleanExpressionTreeNode newParent);

        /// <summary>
        /// Removes the specified child from this node.
        /// Autmatically determines if child is a Left or Right node.
        /// No further action will be taken to ensure tree correctenss.
        /// </summary>
        /// <param name="existingChild"></param>
        void ClearChildAssignment(IBooleanExpressionTreeNode existingChild, out ChildNodePosition childPosition);

        void ClearChildAssignment(IBooleanExpressionTreeNode existingChild);

        /// <summary>
        /// Returns the position (Left or Right) of a child in this node.
        /// </summary>
        /// <param name="existingChild"></param>
        /// <returns></returns>
        ChildNodePosition GetChildPosition(IBooleanExpressionTreeNode existingChild);

        /// <summary>
        /// Assigns the node to Left or Right child position slot.
        /// No further action will be taken to ensure tree correctness.
        /// </summary>
        /// <param name="childNode"></param>
        /// <param name="position"></param>
        void AssignChild(IBooleanExpressionTreeNode childNode, ChildNodePosition position);


        bool IsDescendantOf(IBooleanExpressionTreeNode potentialAncestor);

        void RaiseTreeChanged();
    }
}
