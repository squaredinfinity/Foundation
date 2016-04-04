using SquaredInfinity.Foundation.Collections.Trees;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using SquaredInfinity.Foundation.Presentation.DragDrop;
using System.Threading;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.Collections.BooleanExpressionTree
{
    public class BooleanExpressionTreeViewModel : ViewModel, IDropTarget
    {
        ExampleExpressionTree _expTree = new ExampleExpressionTree();
        public ExampleExpressionTree ExpTree
        {
            get { return _expTree; }
            set { TrySetThisPropertyValue(ref _expTree, value); }
        }

        IDisposable SUBSCRIPTION__AfterExpTreeChanged;

        ExpressionTreeTraversingCollection _flattenedTree;
        public ExpressionTreeTraversingCollection FlattenedTree
        {
            get { return _flattenedTree; }
            set { TrySetThisPropertyValue(ref _flattenedTree, value); }
        }

        public BooleanExpressionTreeViewModel()
        {
            SUBSCRIPTION__AfterExpTreeChanged =
                ExpTree.CreateWeakEventHandler()
                .ForEvent<EventArgs>(
                (s, h) => s.AfterTreeChanged += h,
                (s, h) => s.AfterTreeChanged -= h)
                .Subscribe((_s, _h) =>
                {
                    UpdateFlattenedTree();
                });

            UpdateFlattenedTree();
        }

        public void AddNewNode()
        {
            var new_node = new DummyPredicateNode();
            new_node.Id = ExpTree.GetNextNodeId();

            if (ExpTree.Root == null)
            {
                ExpTree.ReplaceRootAndSubtree(new_node);
                RaisePropertyChanged(nameof(ExpTree));
            }
            else
                ExpTree.InjectInto(new_node, ExpTree.Root);
        }

        public void UpdateFlattenedTree()
        {
            if (ExpTree == null || ExpTree.Root == null)
            {
                FlattenedTree = null;
                return;
            }
            
            var ft = new ExpressionTreeTraversingCollection();
            ft.UpdateRoot(ExpTree.Root);
            FlattenedTree = ft;
        }

        public void RemoveNode(IExpressionTreeNode o)
        {
            ExpTree.RemoveNode(o);
        }

        #region IDropTarget

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            var source = dropInfo.Data as IPredicateConnectiveNode;
            var target = dropInfo.TargetItem as PredicateNode;

            if (source == null || target == null || target.Parent == source)
            {
                dropInfo.AllowedEffects = System.Windows.DragDropEffects.None;
                return;
            }
                

            dropInfo.AllowedEffects = System.Windows.DragDropEffects.Move;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (dropInfo.TargetItem is IPredicateConnectiveNode)
                return;

            var target = dropInfo.TargetItem as IExpressionTreeNode;

            if (target == null)
                return;

            var source = dropInfo.Data as IExpressionTreeNode;

            if (source == null)
                return;

            ExpTree.InjectInto(source, target);
        }

        #endregion
    }

    public class ExampleExpressionTree : ExpressionTree
    {
        int LastNodeId = 0;
        public int GetNextNodeId()
        {
            return Interlocked.Increment(ref LastNodeId);
        }

        public override IPredicateConnectiveNode GetDefaultConnectiveNode()
        {
            var node = new ExamplePredicateConnectiveNode();
            node.Id = GetNextNodeId();
            return node;
        }

        
    }

    public class ExamplePredicateConnectiveNode : PredicateConnectiveNode
    {
        public int Id { get; set; }

        ExampleOperatorTypes _operatorType;
        public ExampleOperatorTypes OperatorType
        {
            get { return _operatorType; }
            set
            {
                _operatorType = value;

                if (value == ExampleOperatorTypes.AND)
                    Operator = new AndOperator();
                else
                    Operator = new OrOperator();

                RaiseTreeChanged();
                RaiseThisPropertyChanged();
            }
        }
        
        public ExamplePredicateConnectiveNode()
        {
            Operator = new AndOperator();
        }

        protected override void DoCopyFrom(IPredicateConnectiveNode other)
        {
            var source = other as ExamplePredicateConnectiveNode;

            source.OperatorType = source.OperatorType;
        }
    }

    public enum ExampleOperatorTypes
    {
        AND,
        OR
    }
    

    public abstract class BinaryOperator : IBinaryOperator
    {
        public abstract bool Evaluate(object payload, IExpressionTreeNode left, IExpressionTreeNode right);
    }

    public class AndOperator : BinaryOperator
    {
        public override bool Evaluate(object payload, IExpressionTreeNode left, IExpressionTreeNode right)
        {
            return
                left.Evaluate(payload)
                &&
                right.Evaluate(payload);
        }
    }

    public class OrOperator : BinaryOperator
    {
        public override bool Evaluate(object payload, IExpressionTreeNode left, IExpressionTreeNode right)
        {
            return
                left.Evaluate(payload)
                ||
                right.Evaluate(payload);
        }
    }

    public class DummyPredicateNode : PredicateNode
    {
        public int Id { get; set; }
        
        public bool Value { get; set; }

        public override bool Evaluate(object payload)
        {
            return Value;
        }
    }
}
