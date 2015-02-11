using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Concurrent;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors
{
    public partial class ContextAwareDataTemplateSelectorService : IContextAwareDataTemplateSelectorService
    {                
        struct Cache
        {
            public string Context;
            public IContextAwareDataTemplateProvider DataTemplateProvider;

            public bool TrySelectTemplate(
                object item,
                DependencyObject container,
                string context,
                bool isTooltip,
                out DataTemplate dataTemplate)
            {
                dataTemplate = null;

                if (DataTemplateProvider == null)
                    return false;

                if (!string.Equals(Context, context))
                    return false;

                return DataTemplateProvider.TrySelectTemplate(item, container, context, isTooltip, out dataTemplate);
            }
        }
    }
}
