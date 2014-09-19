using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Collections.Trees
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class PredicateConnectiveNode : BooleanExpressionTreeNode
    {
        PredicateConnectiveMode _mode;
        public PredicateConnectiveMode Mode 
        {
            get { return _mode; }
            set 
            {
                if (_mode == value)
                    return;

                _mode = value;

                RaiseTreeChanged();
            }
        }

        public override int GetPrecedence()
        {
            if (Mode == PredicateConnectiveMode.AND)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        public override bool Evaluate(object payload)
        {
            if (Mode == PredicateConnectiveMode.AND)
            {
                return
                    Left.Evaluate(payload)
                    &&
                    Right.Evaluate(payload);
            }
            else
            {
                return
                    Left.Evaluate(payload)
                    ||
                    Right.Evaluate(payload);
            }
        }

        public string DebuggerDisplay
        {
            get { return "{0}".FormatWith(Mode); }
        }

        public override IBooleanExpressionTreeNode InjectInto(IBooleanExpressionTreeNode targetNode)
        {
            if (targetNode is PredicateNode)
            {
                return InjectInto(targetNode as PredicateNode);
            }
            else
            {
                return InjectInto(targetNode as PredicateConnectiveNode);
            }
        }

        IBooleanExpressionTreeNode InjectInto(PredicateConnectiveNode targetConnective)
        {
            // todo:

            return FindRoot();
        }

        IBooleanExpressionTreeNode InjectInto(PredicateNode target)
        {
            var source = this;

            var targetParent = target.Parent;

            //# target is child of source, do nothing
            if (object.Equals(target.Parent, source))
                return FindRoot();

            //# target and this are children of same parent -> swap places
            if(targetParent != null && object.Equals(targetParent, source.Parent))
            {
                targetParent.SwapChildren();

                return FindRoot();
            }

            var originalTargetPosition = default(ChildNodePosition);

            if(targetParent != null)
            {
                originalTargetPosition = targetParent.GetChildPosition(target);
            }

            bool targetIsDescendantOfSource = target.IsDescendantOf(source);

            if(targetIsDescendantOfSource)
            {
                // target is a descendant of source
                // before source is injected into a parent of a target (because target is a Predicate Node, source cannot be injected into it directly)
                // attach target parent to parent of source

                var targetGrandParent = targetParent.Parent;

                targetGrandParent.ClearChildAssignment(targetParent);
                targetParent.AssignParent(source.Parent);

                if(source.Parent != null)
                    source.Parent.ReplaceChildNode(source, targetParent);
            }
            else
            {
                if (source.Parent != null)
                {
                    source.Parent.ClearChildAssignment(source);
                    source.AssignParent(null);
                }
            }

            //# check if source has an empty child slot
            if(source.Left == null || source.Right == null)
            {
                // connect target to source
                // connect source to target parent

                source.AssignParent(targetParent);
                targetParent.ReplaceChildNode(target, source);

                target.AssignParent(source);

                if (source.Left == null)
                    source.AssignChild(target, ChildNodePosition.Left);
                else
                    source.AssignChild(target, ChildNodePosition.Right);

                return FindRoot();
            }

            //# source has both children, create a new connective mode to link target parent, target and source

            var newConnective = new PredicateConnectiveNode();

            newConnective.Mode = this.Mode;

            newConnective.AssignChild(target, ChildNodePosition.Left);
            newConnective.AssignChild(this, ChildNodePosition.Right);

            target.AssignParent(newConnective);

            targetParent.AssignChild(newConnective, originalTargetPosition);
            newConnective.AssignParent(targetParent);

            //# update this parent
            if (this.Parent != null)
            {
                this.Parent.ClearChildAssignment(this);
            }

            this.AssignParent(newConnective);

            return FindRoot();
        }
    }
}
