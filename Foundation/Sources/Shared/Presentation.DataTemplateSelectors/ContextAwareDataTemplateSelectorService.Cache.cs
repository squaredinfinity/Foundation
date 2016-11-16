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
            public WeakReference<object> Item;

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

                if (item == null)
                    return false;

                var cached_item = (object)null;

                if(Item.TryGetTarget(out cached_item))
                {
                    if (cached_item == null)
                        return false;

                    if (cached_item.GetType() != item.GetType())
                        return false;
                }

                if (!string.Equals(Context, context))
                    return false;

                return DataTemplateProvider.TrySelectTemplate(item, container, context, isTooltip, out dataTemplate);
            }
        }
    }
}
