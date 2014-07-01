using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors
{
    public interface IContextAwareDataTemplateSelectorService
    {
        void RegisterDataTemplateSelector(IContextAwareDataTemplateSelector selector);

        DataTemplate SelectTemplate(object item, DependencyObject container, string context, bool isTooltip);
    }
}
