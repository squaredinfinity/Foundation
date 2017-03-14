using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Collections.Trees
{
    public interface IExpressionTreeNode
    {
        event EventHandler<EventArgs> AfterTreeChanged;

        IExpressionTreeNode Parent { get; }
        IExpressionTreeNode Left { get; }
        IExpressionTreeNode Right { get; }

        bool Evaluate(object payload);

        /// <summary>
        /// Node with lower precedence is evaluated first
        /// </summary>
        int GetPrecedence();

        IEnumerable<IExpressionTreeNode> TraverseTreeInOrder();
        IEnumerable<IExpressionTreeNode> TraverseTreePostOrder();

        /// <summary>
        /// Inserts specified node as a Left child of this node.
        /// Steps will be taken to ensure tree correctness after this operation.
        /// </summary>
        /// <param name="leftNode"></param>
        void InsertLeft(IExpressionTreeNode leftNode);

        /// <summary>
        /// Inserts specified node as a Right child of this node.
        /// Steps will be taken to ensure tree correctness after this operation.
        /// </summary>
        /// <param name="rightNode"></param>
        void InsertRight(IExpressionTreeNode rightNode);

        /// <summary>
        /// Injects this node into tree hierarchy of specified node.
        /// </summary>
        /// <param name="node"></param>
        IExpressionTreeNode InjectInto(IExpressionTreeNode node, Func<IPredicateConnectiveNode> createConnectiveNode);

        /// <summary>
        /// Replaces old child node with a new child node.
        /// Steps will be taken to ensure tree correctness after this operation.
        /// Old node will be completely removed from the tree.
        /// </summary>
        /// <param name="oldNode"></param>
        /// <param name="newNode"></param>
        void ReplaceChildNode(IExpressionTreeNode oldNode, IExpressionTreeNode newNode);

        void SwapChildren();

        /// <summary>
        /// Assigns a new Parent (or null) to this Node.
        /// No further action will be taken to ensure tree correctness.
        /// </summary>
        /// <param name="newParent"></param>
        void AssignParent(IExpressionTreeNode newParent);

        /// <summary>
        /// Removes the specified child from this node.
        /// Autmatically determines if child is a Left or Right node.
        /// No further action will be taken to ensure tree correctenss.
        /// </summary>
        /// <param name="existingChild"></param>
        void ClearChildAssignment(IExpressionTreeNode existingChild, out ChildNodePosition childPosition);

        void ClearChildAssignment(IExpressionTreeNode existingChild);

        /// <summary>
        /// Returns the position (Left or Right) of a child in this node.
        /// </summary>
        /// <param name="existingChild"></param>
        /// <returns></returns>
        ChildNodePosition GetChildPosition(IExpressionTreeNode existingChild);

        /// <summary>
        /// Assigns the node to Left or Right child position slot.
        /// No further action will be taken to ensure tree correctness.
        /// </summary>
        /// <param name="childNode"></param>
        /// <param name="position"></param>
        void AssignChild(IExpressionTreeNode childNode, ChildNodePosition position);


        bool IsDescendantOf(IExpressionTreeNode potentialAncestor);

        void RaiseTreeChanged();
    }
}
