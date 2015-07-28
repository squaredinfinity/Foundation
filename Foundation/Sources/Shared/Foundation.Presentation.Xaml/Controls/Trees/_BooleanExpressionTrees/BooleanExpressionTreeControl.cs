using SquaredInfinity.Foundation.Collections.Trees;
using SquaredInfinity.Foundation.Presentation.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SquaredInfinity.Foundation.Presentation.Controls.Trees
{
    public class BooleanExpressionTreeControl : Control, IDropTarget, IDragSource
    {
        #region Tree

        public ExpressionTree Tree
        {
            get { return (ExpressionTree)GetValue(TreeProperty); }
            set { SetValue(TreeProperty, value); }
        }

        public static readonly DependencyProperty TreeProperty =
            DependencyProperty.Register(
            "Tree",
            typeof(ExpressionTree),
            typeof(BooleanExpressionTreeControl),
            new PropertyMetadata(null, new PropertyChangedCallback(OnTreeChanged)));

        static void OnTreeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tc = d as BooleanExpressionTreeControl;

            if (tc == null)
                return;

            var tree = tc.Tree;

            if (tree == null)
            {
                tc.FlattenedTree.UpdateRoot(root: null);
            }
            else
            {
                tc.FlattenedTree.UpdateRoot(tc.Tree.Root);
            }
        }

        #endregion

        #region Flattened Tree

        public ExpressionTreeTraversingCollection FlattenedTree
        {
            get { return (ExpressionTreeTraversingCollection)GetValue(FlattenedTreeProperty); }
            set { SetValue(FlattenedTreeProperty, value); }
        }

        public static readonly DependencyProperty FlattenedTreeProperty =
            DependencyProperty.Register(
            "FlattenedTree",
            typeof(ExpressionTreeTraversingCollection),
            typeof(BooleanExpressionTreeControl),
            new PropertyMetadata(new ExpressionTreeTraversingCollection()));

        #endregion

        #region IDropTarget

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data == dropInfo.TargetItem)
                dropInfo.AllowedEffects = DragDropEffects.None;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            var target = dropInfo.TargetItem as IExpressionTreeNode;

            if (target == null)
                return;

            var source = dropInfo.Data as IExpressionTreeNode;

            if (source == null)
                return;

            if (source is PredicateNode && target is PredicateNode)
            {
                Tree.InjectInto(source, target);
            }
            else
            {
                throw new NotSupportedException();
            }
            

            // todo: do this as XamlVirutalizingCollection

            var ft = new ExpressionTreeTraversingCollection();
            ft.UpdateRoot(Tree.Root);
            FlattenedTree = ft;
            //FlattenedTree.UpdateRoot(Tree.Root);
        }

        #endregion

        #region IDragSource

        void IDragSource.StartDrag(IDragInfo dragInfo)
        {
            dragInfo.AllowedEffects = DragDropEffects.Move;
            dragInfo.Data = dragInfo.SourceItem;
        }

        bool IDragSource.CanStartDrag(IDragInfo dragInfo)
        {
            if (dragInfo.SourceItem is PredicateNode)
                return true;

            return false;
        }

        void IDragSource.Dropped(IDropInfo dropInfo)
        {

        }

        void IDragSource.DragCancelled()
        {

        }

        #endregion

    }
}
