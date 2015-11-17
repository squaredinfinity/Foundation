using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Collections.Trees;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.Collections.BooleanExpressionTree
{
    public class PredicateNodeViewModel : ExpressionTreeViewModel<PredicateNode>
    {
        protected override void OnAfterDataContextChanged(PredicateNode newDataContext)
        {
            base.OnAfterDataContextChanged(newDataContext);
        }
    }
}
