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
        void RegisterDataTemplateSelector(IContextAwareDataTemplateProvider selector, string context = null);

        DataTemplate SelectTemplate(object item, DependencyObject container, string context, bool isTooltip);
    }
}
