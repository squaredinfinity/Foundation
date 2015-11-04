using SquaredInfinity.Foundation.Presentation.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests._internal
{
    public class GenericView : View<GenericViewModel>
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Trace.WriteLine("");
        }
    }
}
