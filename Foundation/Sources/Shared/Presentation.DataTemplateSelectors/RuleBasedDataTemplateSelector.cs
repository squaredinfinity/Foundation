using SquaredInfinity.Presentation.DataTemplateSelectors.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SquaredInfinity.Presentation.DataTemplateSelectors
{
    [ContentProperty("Rules")]
    public class RuleBasedDataTemplateSelector : DataTemplateSelector
    {
        List<IDataTemplateSelectorRule> _rules = new List<IDataTemplateSelectorRule>();
        public List<IDataTemplateSelectorRule> Rules
        {
            get { return _rules; }
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var dt = (DataTemplate)null;

            foreach(var rule in Rules)
            {
                if(rule.TryEvaluate(item, container, out dt))
                {
                    return dt;
                }
            }

            return dt;
        }
    }
}
