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
        ILock UpdateLock = new ReaderWriterLockSlimEx();
                
        readonly Dictionary<string, IList<IContextAwareDataTemplateProvider>> ByContext 
            = new Dictionary<string, IList<IContextAwareDataTemplateProvider>>();
        
        public virtual void RegisterDataTemplateSelector(IContextAwareDataTemplateProvider selector, string context = null)
        {
            if(context == null)
                context = "";

            using (UpdateLock.AcquireWriteLock())
            {
                if(!ByContext.ContainsKey(context))
                {
                    ByContext.Add(context, new List<IContextAwareDataTemplateProvider>());
                }
                
                ByContext[context].Add(selector);
            }
        }

        public virtual DataTemplate SelectTemplate(object item, DependencyObject container, string context, bool isTooltip)
        {
            var dt = (DataTemplate)null;

            if (context == null)
                context = "";

            using (UpdateLock.AcquireReadLock())
            {
                // check this context specific selectors first

                if (ByContext.ContainsKey(context))
                {
                    var selectors_by_context = ByContext[context];

                    for (int i = 0; i < selectors_by_context.Count; i++)
                    {
                        var selector = selectors_by_context[i];

                        if (selector.TrySelectTemplate(item, container, context, isTooltip, out dt))
                        {
                            return dt;
                        }
                    }
                }

                // check '*' selectors
                if(ByContext.ContainsKey("*"))
                {
                    var selectors_by_context = ByContext["*"];

                    for (int i = 0; i < selectors_by_context.Count; i++)
                    {
                        var selector = selectors_by_context[i];

                        if (selector.TrySelectTemplate(item, container, context, isTooltip, out dt))
                        {
                            return dt;
                        }
                    }
                }
            }

            return null;
        }
    }
}
