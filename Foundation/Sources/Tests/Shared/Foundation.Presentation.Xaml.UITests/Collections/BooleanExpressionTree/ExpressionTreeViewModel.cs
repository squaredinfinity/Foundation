using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Collections.Trees;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.Collections.BooleanExpressionTree
{
    public class ExpressionTreeViewModel<TNode> : ViewModel<TNode>
        where TNode : ExpressionTreeNode
    {
        ObservableCollectionEx<int> _bracketIndentLevels = new ObservableCollectionEx<int>();
        public ObservableCollectionEx<int> BracketIndentLevels
        {
            get { return _bracketIndentLevels; }
        }

        public int? BracketIndentLevel
        {
            get
            {
                if (BracketIndentLevels.Count > 0)
                    return BracketIndentLevels[BracketIndentLevels.Count - 1] + 1;

                return 0;
            }
        }

        protected override void OnAfterDataContextChanged(TNode newDataContext)
        {
            base.OnAfterDataContextChanged(newDataContext);
            
            BracketIndentLevels.Clear();

            if (newDataContext != null)
            {
                int bracketIndentLevel = 0;
                int indentLevel = 0;

                newDataContext.CalculateIndentLevels(out bracketIndentLevel, out indentLevel);

                for (int i = 0; i < indentLevel; i++)
                {
                    BracketIndentLevels.Add(i);
                }
            }
        }
    }
}
