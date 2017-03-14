using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using SquaredInfinity.ComponentModel;

namespace SquaredInfinity.Collections.Trees
{
    public abstract class ExpressionTree : NotifyPropertyChangedObject
    {
        public event EventHandler<EventArgs> AfterTreeChanged;

        IExpressionTreeNode _root;
        public IExpressionTreeNode Root 
        {
            get { return _root; }
            set
            {
                var _old = _root;

                if(_old != null)
                {
                    _old.AfterTreeChanged -= HandleAfterTreeChanged;
                }

                _root = value;

                if(_root != null)
                {
                    _root.AfterTreeChanged -= HandleAfterTreeChanged;
                    _root.AfterTreeChanged += HandleAfterTreeChanged;
                }

                RaiseThisPropertyChanged();
            }
        }

        void HandleAfterTreeChanged(object sender, EventArgs e)
        {
            RaiseTreeChanged();
        }

        public bool Evaluate(object payload)
        {
            if (Root == null)
                return true;

            return Root.Evaluate(payload);
        }

        void RaiseTreeChanged()
        {
            if (AfterTreeChanged != null)
                AfterTreeChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Cleans up the tree structure to ensure that all Connective Nodes have both children assigned.
        /// If a Connective has no children assigned, it will be removed.
        /// If a Connective has one child assigned, it will be removed and child moved in its place.
        /// </summary>
        void CleanUpTreeStructure()
        {
            if (Root == null)
                return;

            //# find te root

            var root_candidate = Root;

            foreach (var node in Root.TraverseTreePostOrder())
            {
                if (node is PredicateNode)
                    continue;

                var connectiveNode = node as PredicateConnectiveNode;

                //# Connective Node with both children => do nothing
                if (connectiveNode.Left != null && connectiveNode.Right != null)
                    continue;

                //# Connective Node without any children => REMOVE
                if (connectiveNode.Left == null && connectiveNode.Right == null)
                {
                    if (connectiveNode.Parent == null)
                    {
                        // this is the root, replace it with null
                        Root = null;
                        break;
                    }
                    else
                    {
                        connectiveNode.Parent.ReplaceChildNode(connectiveNode, null);
                        continue;
                    }
                }

                //# Connective Node with only one child => replace connective node with the child
                if (connectiveNode.Left == null)
                {
                    if (connectiveNode.Parent == null)
                    {
                        // this is the root, replace it with Right Child
                        Root = connectiveNode.Right;
                        Root.AssignParent(newParent: null);

                    }
                    else
                    {
                        connectiveNode.Parent.ReplaceChildNode(connectiveNode, connectiveNode.Right);
                        continue;
                    }
                }

                if (connectiveNode.Right == null)
                {
                    if (connectiveNode.Parent == null)
                    {
                        // this is the root, replace it with Left Child
                        Root = connectiveNode.Left;
                        Root.AssignParent(newParent: null);
                    }
                    else
                    {
                        connectiveNode.Parent.ReplaceChildNode(connectiveNode, connectiveNode.Left);
                        continue;
                    }
                }
            }

            RaiseTreeChanged();
        }

        public void InjectInto(IExpressionTreeNode source, IExpressionTreeNode target)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (target == null)
                throw new ArgumentNullException("target");

            //# cannot insert parent into its child
            if (target.Parent == source)
                return;

            Root = source.InjectInto(target, GetDefaultConnectiveNode);

            CleanUpTreeStructure();

            RaiseTreeChanged();
        }

        public void RemoveNode(IExpressionTreeNode node)
        {
            var nodeAsPredicate = node as PredicateNode;

            if (nodeAsPredicate == null)
                throw new NotSupportedException();

            RemoveNodeInternal(nodeAsPredicate);

            CleanUpTreeStructure();
            RaiseTreeChanged();
        }

        void RemoveNodeInternal(PredicateNode node)
        {
            if (node == Root)
            {
                ReplaceRootAndSubtree(null);
                return;
            }

            var nodeParent = node.Parent;

            var position = nodeParent.GetChildPosition(node);

            nodeParent.AssignChild(null, position);
        }

        public void ReplaceRootAndSubtree(IExpressionTreeNode newRootAndSubTree)
        {
            Root = newRootAndSubTree;
            CleanUpTreeStructure();
            RaiseTreeChanged();
        }

        public void AppendNode(IExpressionTreeNode newNode)
        {
            if (Root == null)
            {
                ReplaceRootAndSubtree(newNode);
            }
            else
            {
                InjectInto(newNode, Root);
            }
        }

        public abstract IPredicateConnectiveNode GetDefaultConnectiveNode();
    }
}
