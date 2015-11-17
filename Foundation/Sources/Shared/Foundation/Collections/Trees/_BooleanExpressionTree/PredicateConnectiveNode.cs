using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Collections.Trees
{
    public interface IBinaryOperator
    {
        bool Evaluate(object payload, IExpressionTreeNode left, IExpressionTreeNode right);
    }

    public interface IPredicateConnectiveNode : IExpressionTreeNode
    {
        IBinaryOperator Operator { get; set; }

        void CopyFrom(IPredicateConnectiveNode other);
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public abstract class PredicateConnectiveNode : ExpressionTreeNode, IPredicateConnectiveNode
    {
        IBinaryOperator _operator;
        public IBinaryOperator Operator
        {
            get { return _operator; }
            set { TrySetThisPropertyValue(ref _operator, value); }
        }

        public void CopyFrom(IPredicateConnectiveNode other)
        {
            DoCopyFrom(other);
        }

        protected abstract void DoCopyFrom(IPredicateConnectiveNode other);



        public override bool Evaluate(object payload)
        {
            return Operator.Evaluate(payload, Left, Right);
        }

        public string DebuggerDisplay
        {
            get { return "{0}".FormatWith(Operator.ToString(valueWhenNull: "[Null]")); }
        }

        public override IExpressionTreeNode InjectInto(IExpressionTreeNode targetNode, Func<IPredicateConnectiveNode> createConnectiveNode)
        {
            if (targetNode is PredicateNode)
            {
                return InjectInto(targetNode as PredicateNode, createConnectiveNode);
            }
            else
            {
                return InjectInto(targetNode as PredicateConnectiveNode, createConnectiveNode);
            }
        }

        IExpressionTreeNode InjectInto(PredicateConnectiveNode targetConnective, Func<IPredicateConnectiveNode> createConnectiveNode)
        {
            // todo:

            return FindRoot();
        }

        IExpressionTreeNode InjectInto(PredicateNode target, Func<IPredicateConnectiveNode> createConnectiveNode)
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

            var newConnective = createConnectiveNode();

            newConnective.CopyFrom(this);

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
