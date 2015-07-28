using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors.Rules
{
    public abstract class DataTemplateSelectorRule : IDataTemplateSelectorRule
    {
        public DataTemplate DataTemplate { get; set; }

        public abstract bool TryEvaluate(object item, DependencyObject container, out DataTemplate dt);
    }
}
