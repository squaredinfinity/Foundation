using SquaredInfinity.Foundation.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections.Trees
{
    public abstract class ExpressionTreeNode : NotifyPropertyChangedObject, IExpressionTreeNode
    {
        IExpressionTreeNode _parent;
        public IExpressionTreeNode Parent
        {
            get { return _parent; }
            set { TrySetThisPropertyValue(ref _parent, value); }
        }

        IExpressionTreeNode _left;
        public IExpressionTreeNode Left
        {
            get { return _left; }
            set { TrySetThisPropertyValue(ref _left, value); }
        }

        IExpressionTreeNode _right;
        public IExpressionTreeNode Right
        {
            get { return _right; }
            set { TrySetThisPropertyValue(ref _right, value); }
        }

        public virtual int GetPrecedence() { return 0; }

        public abstract bool Evaluate(object payload);

        public virtual void ReplaceChildNode(IExpressionTreeNode oldNode, IExpressionTreeNode newNode)
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


        public virtual void InsertLeft(IExpressionTreeNode leftNode)
        {
            if (leftNode == null)
                throw new ArgumentNullException("leftNode");

            if (Left != null)
                throw new InvalidOperationException("This node already contains left child node.");

            Left = leftNode;
            Left.AssignParent(this);

            RaiseTreeChanged();
        }

        public virtual void InsertRight(IExpressionTreeNode rightNode)
        {
            if (rightNode == null)
                throw new ArgumentNullException("rightNode");

            if (Right != null)
                throw new InvalidOperationException("This node already contains right child node.");

            Right = rightNode;
            Right.AssignParent(this);

            RaiseTreeChanged();
        }


        public void AssignParent(IExpressionTreeNode newParent)
        {
            Parent = newParent;
        }

        public abstract IExpressionTreeNode InjectInto(IExpressionTreeNode node, Func<IPredicateConnectiveNode> createConnectiveNode);

        public void ClearChildAssignment(IExpressionTreeNode existingChild)
        {
            ChildNodePosition childPosition_ignored = default(ChildNodePosition);

            ClearChildAssignment(existingChild, out childPosition_ignored);
        }

        public void ClearChildAssignment(IExpressionTreeNode existingChild, out ChildNodePosition childPosition)
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


        public ChildNodePosition GetChildPosition(IExpressionTreeNode existingChild)
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

        public void AssignChild(IExpressionTreeNode childNode, ChildNodePosition position)
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

        public IEnumerable<IExpressionTreeNode> TraverseTreeInOrder()
        {
            Stack<IExpressionTreeNode> workQueue = new Stack<IExpressionTreeNode>();

            IExpressionTreeNode node = this;

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

        public IEnumerable<IExpressionTreeNode> TraverseTreePostOrder()
        {
            Stack<IExpressionTreeNode> workQueue = new Stack<IExpressionTreeNode>();

            IExpressionTreeNode node = this;
            IExpressionTreeNode lastVisited = null;

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
            var lastVisitedParent = (IExpressionTreeNode)this;

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
                else if(lastVisitedParentAsConnective != null && parentAsConnective != null && lastVisitedParentAsConnective.Operator.GetType() != parentAsConnective.Operator.GetType())
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

        public bool IsDescendantOf(IExpressionTreeNode potentialAncestor)
        {
            var ancestor = (IExpressionTreeNode)Parent;

            while (ancestor != null)
            {
                if (ancestor == potentialAncestor)
                    return true;

                ancestor = ancestor.Parent;
            }

            return false;
        }

        public IExpressionTreeNode FindRoot()
        {
            var root_candidate = (IExpressionTreeNode)this;

            while (root_candidate.Parent != null)
                root_candidate = root_candidate.Parent;

            return root_candidate;
        }
    }
}
