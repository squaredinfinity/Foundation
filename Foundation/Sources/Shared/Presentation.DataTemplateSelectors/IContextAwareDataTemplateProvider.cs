using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors
{
    /// <summary>
    /// Provides a logic for selecting Data Type for given item when placed in an application in given context.
    /// Context is provided by the host application (e.g. SearchResults, RecentItems etc.) and can be used by Type Data Template Selector
    /// to choose specific template to be used.
    /// </summary>
    public interface IContextAwareDataTemplateProvider
    {
        bool TrySelectTemplate(
            object item,
            DependencyObject container,
            string context,
            bool isTooltip,
            out DataTemplate dataTemplate);
    }
}
