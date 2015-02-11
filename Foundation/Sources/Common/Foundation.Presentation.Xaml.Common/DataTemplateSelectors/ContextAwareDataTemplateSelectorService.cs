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
    public class ContextAwareDataTemplateSelectorService : IContextAwareDataTemplateSelectorService
    {                
        readonly Dictionary<string, IList<IContextAwareDataTemplateProvider>> ByContext 
            = new Dictionary<string, IList<IContextAwareDataTemplateProvider>>();

        public virtual void RegisterDataTemplateSelector(IContextAwareDataTemplateProvider selector, string context = null)
        {
            if (context == null)
                context = "";

            if (!ByContext.ContainsKey(context))
            {
                ByContext.Add(context, new List<IContextAwareDataTemplateProvider>());
            }

            ByContext[context].Add(selector);
        }

        public virtual DataTemplate SelectTemplate(object item, DependencyObject container, string context, bool isTooltip)
        {
            var dt = (DataTemplate)null;

            // check this context specific selectors first

            var selectors_by_context = (IList<IContextAwareDataTemplateProvider>)null;

            if (ByContext.TryGetValue(context, out selectors_by_context))
            {
                for (int i = 0; i < selectors_by_context.Count; i++)
                {
                    if (selectors_by_context[i].TrySelectTemplate(item, container, context, isTooltip, out dt))
                    {
                        return dt;
                    }
                }
            }

            // check '*' selectors
            if (ByContext.TryGetValue("*", out selectors_by_context))
            {
                for (int i = 0; i < selectors_by_context.Count; i++)
                {
                    if (selectors_by_context[i].TrySelectTemplate(item, container, context, isTooltip, out dt))
                    {
                        return dt;
                    }
                }
            }

            //! Returning null here may cause StackOverflowException in WPF framework
            //  Return empty dataTemplate instead
            return new DataTemplate();
        }
    }
}
