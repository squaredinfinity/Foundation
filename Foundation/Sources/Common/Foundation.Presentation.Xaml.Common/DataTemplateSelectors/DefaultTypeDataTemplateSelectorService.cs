using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors
{
    public class DefaultTypeDataTemplateSelectorService : IContextAwareDataTemplateSelectorService
    {
        ILock UpdateLock = new ReaderWriterLockSlimEx();

        readonly List<IContextAwareDataTemplateSelector> Selectors = new List<IContextAwareDataTemplateSelector>();

        bool IsCacheDirty = true;

        public void RegisterDataTemplateSelector(IContextAwareDataTemplateSelector selector)
        {
            using (UpdateLock.AcquireWriteLock())
            {
                Selectors.Add(selector);
            }
        }

        public DataTemplate SelectTemplate(object item, DependencyObject container, string context, bool isTooltip)
        {
            var dt = (DataTemplate)null;

            using (UpdateLock.AcquireReadLock())
            {
                if (IsCacheDirty)
                    RefreshCache();

                if (!ByContext.ContainsKey(context))
                    return null;

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

            return null;
        }

        Dictionary<string, IReadOnlyList<IContextAwareDataTemplateSelector>> ByContext;

        void RefreshCache()
        {
            ByContext = new Dictionary<string, IReadOnlyList<IContextAwareDataTemplateSelector>>();

            var allContexts =
                (from s in Selectors
                 from c in s.GetSupportedContexts()
                 select c).Distinct().ToArray();

            foreach(var context in allContexts)
            {
                var selectors_by_context =
                    (from s in Selectors
                     where s.GetSupportedContexts().Contains(context)
                     select s);

                ByContext.Add(context, selectors_by_context.ToArray());
            }
            
        }
    }
}
