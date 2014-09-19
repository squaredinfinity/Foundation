using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Collections.Trees
{
    public abstract class PredicateNode : BooleanExpressionTreeNode
    {
        public override IBooleanExpressionTreeNode InjectInto(IBooleanExpressionTreeNode node)
        {
            if(node is PredicateNode)
            {
                return InjectInto(node as PredicateNode);
            }
            else
            {
                return InjectInto(node as PredicateConnectiveNode);
            }
        }

        IBooleanExpressionTreeNode InjectInto(PredicateConnectiveNode targetConnective)
        {
            if (targetConnective == null)
                throw new ArgumentNullException("node");
            
            return InjectInto(this, targetConnective);
        }

        IBooleanExpressionTreeNode InjectInto(PredicateNode targetPredicate)
        {
            if (targetPredicate == null)
                throw new ArgumentNullException("targetPredicate");

            var sourceParent = Parent;

            var targetParent = targetPredicate.Parent;

            if(Parent != null && targetParent != null && object.ReferenceEquals(Parent, targetParent))
            {
                Parent.SwapChildren();
                return FindRoot();

            }

            if(targetParent == null)
            {
                // create a parent for a target
                var newParent = new PredicateConnectiveNode();
                newParent.AssignChild(targetPredicate, ChildNodePosition.Right);
                targetPredicate.AssignParent(newParent);

                targetParent = newParent;
            }

            //# get target sibling
            //  injection will be handled differently depending on type of the sibling
            var targetPosition = targetParent.GetChildPosition(targetPredicate);

            var targetSibling = (IBooleanExpressionTreeNode)null;

            if (targetPosition == ChildNodePosition.Left)
                targetSibling = targetParent.Right;
            else
                targetSibling = targetParent.Left;

            if (targetSibling == null || targetSibling is PredicateNode)
            {
                // target sibling is null or predicate
                // we can simply replace target with a Connective Node
                // (if sibling was a connective node itself then injecting another connective node would create bracket in logic, which we don't want)
                // (node with two connective node children is treated as a bracket, e.g: AND.l = [a OR b], AND.l = [c OR d] => ((a OR b) AND (c OR d))

                return InjectInto(this, targetPredicate);
            }
            else if (targetSibling is PredicateConnectiveNode)
            {
                // target sibling is a Predicate Connective Node
                // predicate will be injected in a new Predicate Connective Node above target parent

                return InjectInto(this, targetParent as PredicateConnectiveNode);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">Node to be injected</param>
        /// <param name="target">Target Node which will be replaced by Source, and joined with source's new sub-tree</param>
        static IBooleanExpressionTreeNode InjectInto(IBooleanExpressionTreeNode source, IBooleanExpressionTreeNode target)
        {
            var sourceParent = source.Parent;

            var sourceParentAsConnective = sourceParent as PredicateConnectiveNode;

            var sourceSibling = (IBooleanExpressionTreeNode)null;

            if (sourceParentAsConnective != null)
            {
                var sourcePosition = sourceParentAsConnective.GetChildPosition(source);

                if (sourcePosition == ChildNodePosition.Left)
                    sourceSibling = sourceParentAsConnective.Right;
                else
                    sourceSibling = sourceParentAsConnective.Left;
            }
            var sourceSiblingAsConnective = sourceSibling as PredicateConnectiveNode;
            
            var targetParent = target.Parent;

            var targetParentAsConnective = targetParent as PredicateConnectiveNode;

            var targetSibling = (IBooleanExpressionTreeNode)null;

            var targetPosition = ChildNodePosition.Right; // default should be right, if target has no parent

            if(targetParent != null)
            {
                targetPosition = targetParent.GetChildPosition(target);
            }

            if (targetParentAsConnective != null)
            {
                if (targetPosition == ChildNodePosition.Left)
                    targetSibling = targetParentAsConnective.Right;
                else
                    targetSibling = targetParentAsConnective.Left;
            }

            var targetSiblingAsConnective = targetSibling as PredicateConnectiveNode;

            var newConnective = new PredicateConnectiveNode();

            if (targetParent != null)
            {
                //# remove target from its parent
                target.AssignParent(newParent: null);
                targetParent.ClearChildAssignment(target);

                //# assign new Connective in place of target
                targetParent.AssignChild(newConnective, targetPosition);
                newConnective.AssignParent(targetParent);
            }
            else
            {

            }

            //# assign old target as left child node of new connective
            if (targetPosition == ChildNodePosition.Left)
                newConnective.AssignChild(target, ChildNodePosition.Right);
            else
                newConnective.AssignChild(target, ChildNodePosition.Left);

            //# copy connective mode from target parent (if exists)
            if (targetParentAsConnective != null)
                newConnective.Mode = targetParentAsConnective.Mode;
            else if (targetSiblingAsConnective != null)
                newConnective.Mode = targetSiblingAsConnective.Mode;

            target.AssignParent(newConnective);

            //# assign source node as right child node of new connective
            newConnective.AssignChild(source, targetPosition);
            source.AssignParent(newConnective);

            //# fix original parent tree
            //  parent is an empty connective or connective with one child now
            //  replace parent connective with its child (removing the connective completely from the tree)
            if (sourceParent != null)
            {
                //# remove source from its parent
                var sourcePosition = default(ChildNodePosition);
                sourceParent.ClearChildAssignment(source, out sourcePosition);

                //# get other source parent child
                var oldSibling = (IBooleanExpressionTreeNode)null;
                if (sourcePosition == ChildNodePosition.Left)
                    oldSibling = sourceParent.Right;
                else
                    oldSibling = sourceParent.Left;

                //# in grand parent, replace source parent with its other sibling
                var grandParent = sourceParent.Parent;

                if (grandParent != null)
                    grandParent.ReplaceChildNode(sourceParent, oldSibling);
            }

            //# get new root element of a tree

            var root_candidate = (IBooleanExpressionTreeNode)newConnective;
            while (root_candidate.Parent != null)
                root_candidate = root_candidate.Parent;

            return root_candidate;
        }
    }
}
