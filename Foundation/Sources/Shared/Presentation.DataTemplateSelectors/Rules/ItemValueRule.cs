using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Presentation.DataTemplateSelectors.Rules
{
    public class ItemValueRule : DataTemplateSelectorRule
    {
        public object ExpectedValue { get; set; }

        public override bool TryEvaluate(object item, DependencyObject container, out System.Windows.DataTemplate dt)
        {
            dt = DataTemplate;

            return object.Equals(ExpectedValue, item);
        }
    }
}
