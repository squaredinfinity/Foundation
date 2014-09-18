using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections.Trees
{
    public abstract class BooleanExpressionTreeNode : IBooleanExpressionTreeNode
    {
        public IBooleanExpressionTreeNode Parent { get; set; }

        public IBooleanExpressionTreeNode Left { get; set; }

        public IBooleanExpressionTreeNode Right { get; set; }

        public virtual int GetPrecedence() { return 0; }

        public abstract bool Evaluate(object payload);

        public virtual void ReplaceChildNode(IBooleanExpressionTreeNode oldNode, IBooleanExpressionTreeNode newNode)
        {
            if (Left == oldNode)
            {
                Left = newNode;
            }
            else if (Right == oldNode)
            {
                Right = newNode;
            }
            else
            {
                throw new InvalidOperationException("old node is not a child of this element");
            }

            if (newNode != null)
            {
                newNode.AssignParent(this);
            }
        }


        public virtual void InsertLeft(IBooleanExpressionTreeNode leftNode)
        {
            if (leftNode == null)
                throw new ArgumentNullException("leftNode");

            if (Left != null)
                throw new InvalidOperationException("This node already contains left child node.");

            Left = leftNode;
            Left.AssignParent(this);

            RaiseTreeChanged();
        }

        public virtual void InsertRight(IBooleanExpressionTreeNode rightNode)
        {
            if (rightNode == null)
                throw new ArgumentNullException("rightNode");

            if (Right != null)
                throw new InvalidOperationException("This node already contains right child node.");

            Right = rightNode;
            Right.AssignParent(this);

            RaiseTreeChanged();
        }


        public void AssignParent(IBooleanExpressionTreeNode newParent)
        {
            Parent = newParent;
        }

        public abstract IBooleanExpressionTreeNode InjectInto(IBooleanExpressionTreeNode node);

        public void ClearChildAssignment(IBooleanExpressionTreeNode existingChild)
        {
            ChildNodePosition childPosition_ignored = default(ChildNodePosition);

            ClearChildAssignment(existingChild, out childPosition_ignored);
        }

        public void ClearChildAssignment(IBooleanExpressionTreeNode existingChild, out ChildNodePosition childPosition)
        {
            if (object.Equals(Left, existingChild))
            {
                Left = null;
                childPosition = ChildNodePosition.Left;
            }
            else if (object.Equals(Right, existingChild))
            {
                Right = null;
                childPosition = ChildNodePosition.Right;
            }
            else
            {
                throw new ArgumentException("Specified node is not a child of this node.");
            }
        }


        public ChildNodePosition GetChildPosition(IBooleanExpressionTreeNode existingChild)
        {
            if (object.Equals(Left, existingChild))
            {
                return ChildNodePosition.Left;
            }
            else if (object.Equals(Right, existingChild))
            {
                return ChildNodePosition.Right;
            }
            else
            {
                throw new ArgumentException("Specified node is not a child of this node.");
            }
        }

        public void AssignChild(IBooleanExpressionTreeNode childNode, ChildNodePosition position)
        {
            if(position == ChildNodePosition.Left)
            {
                Left = childNode;
            }
            else
            {
                Right = childNode;
            }
        }

        public IEnumerable<IBooleanExpressionTreeNode> TraverseTreeInOrder()
        {
            Stack<IBooleanExpressionTreeNode> workQueue = new Stack<IBooleanExpressionTreeNode>();

            IBooleanExpressionTreeNode node = this;

            while (workQueue.Count != 0 || node != null)
            {
                if (node != null)
                {
                    workQueue.Push(node);
                    node = node.Left;
                    continue;
                }
                else
                {
                    node = workQueue.Pop();
                    yield return node;
                    node = node.Right;
                }
            }
        }

        public IEnumerable<IBooleanExpressionTreeNode> TraverseTreePostOrder()
        {
            Stack<IBooleanExpressionTreeNode> workQueue = new Stack<IBooleanExpressionTreeNode>();

            IBooleanExpressionTreeNode node = this;
            IBooleanExpressionTreeNode lastVisited = null;

            while (workQueue.Count != 0 || node != null)
            {
                if (node != null)
                {
                    workQueue.Push(node);
                    node = node.Left;
                    continue;
                }
                else
                {
                    var peek = workQueue.Peek();

                    if (peek.Right != null && lastVisited != peek.Right)
                    {
                        node = peek.Right;
                    }
                    else
                    {
                        workQueue.Pop();
                        yield return peek;
                        lastVisited = peek;
                    }
                }
            }
        }

        public void CalculateIndentLevels(out int bracketIndentLevel, out int indentLevel)
        {
            int finalIndentLevel = 0;

            var parent = Parent;
            var lastVisitedParent = (IBooleanExpressionTreeNode)this;

            while (parent != null)
            {
                var parentLeftAsConnective = parent.Left as PredicateConnectiveNode;
                var parentRightAsConnective = parent.Right as PredicateConnectiveNode;

                var parentAsConnective = parent as PredicateConnectiveNode;
                var lastVisitedParentAsConnective = lastVisitedParent as PredicateConnectiveNode;

                // if parent has both children of Predicate Connective Node type, then it is treated as a brackets
                if (parentLeftAsConnective != null && parentRightAsConnective != null)
               // if(parent.Left != null && parent.Right != null && parent.Left.GetType() == parent.Right.GetType())
                {
                    finalIndentLevel++;
                }
                else if(lastVisitedParentAsConnective != null && parentAsConnective != null && lastVisitedParentAsConnective.Mode != parentAsConnective.Mode)
                {
                    finalIndentLevel++;
                }
                else
                {
                    // nothing to do here
                }

                lastVisitedParent = parent;
                parent = parent.Parent;
            }

            bracketIndentLevel = finalIndentLevel;

            if (this is PredicateNode)
                finalIndentLevel++;

            indentLevel = finalIndentLevel;
        }


        public event EventHandler<EventArgs> AfterTreeChanged;

        public void RaiseTreeChanged()
        {
            if (AfterTreeChanged != null)
                AfterTreeChanged(this, EventArgs.Empty);

            if (Parent != null)
                Parent.RaiseTreeChanged();
        }


        public void SwapChildren()
        {
            var left = Left;
            var right = Right;

            Left = right;
            Right = left;
        }

        public bool IsDescendantOf(IBooleanExpressionTreeNode potentialAncestor)
        {
            var ancestor = (IBooleanExpressionTreeNode)Parent;

            while (ancestor != null)
            {
                if (ancestor == potentialAncestor)
                    return true;

                ancestor = ancestor.Parent;
            }

            return false;
        }
    }
}
