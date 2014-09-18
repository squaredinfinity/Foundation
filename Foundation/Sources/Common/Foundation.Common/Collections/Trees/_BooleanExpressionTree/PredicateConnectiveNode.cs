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
            var targetParent = target.Parent;

            //# target and this are children of same parent -> swap places
            if(targetParent != null && targetParent == this.Parent)
            {
                targetParent.SwapChildren();

                return FindRoot();
            }

            //# replace target with Connective Node
            //  add target as left
            //  add this as right

            var targetPosition = targetParent.GetChildPosition(target);

            var newConnective = new PredicateConnectiveNode();
            newConnective.Mode = PredicateConnectiveMode.OR;

            newConnective.AssignChild(target, ChildNodePosition.Left);
            newConnective.AssignChild(this, ChildNodePosition.Right);

            target.AssignParent(newConnective);

            targetParent.AssignChild(newConnective, targetPosition);
            newConnective.AssignParent(targetParent);

            //# update this parent
            if(this.Parent != null)
            {
                this.Parent.ClearChildAssignment(this);
            }

            this.AssignParent(newConnective);

            return FindRoot();
        }

        IBooleanExpressionTreeNode FindRoot()
        {
            var root_candidate = (IBooleanExpressionTreeNode) this;
            
            while (root_candidate.Parent != null)
                root_candidate = root_candidate.Parent;

            return root_candidate;
        }
    }
}
