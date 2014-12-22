using SquaredInfinity.Foundation.Presentation.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Foundation.Presentation.Xaml.UITests.UIExamples.ToolTip
{
    public class ExampleTooltipView : View<ExampleTooltipViewModel>
    {
        public ExampleTooltipView()
        {
            Loaded += ExampleTooltipView_Loaded;
        }

        void ExampleTooltipView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
