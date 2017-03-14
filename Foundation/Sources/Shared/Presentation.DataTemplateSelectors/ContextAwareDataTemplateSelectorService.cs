using SquaredInfinity.Collections;
using SquaredInfinity.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SquaredInfinity.Extensions;
using System.Collections.Concurrent;

namespace SquaredInfinity.Presentation.DataTemplateSelectors
{
    public partial class ContextAwareDataTemplateSelectorService : IContextAwareDataTemplateSelectorService
    {                
        readonly Dictionary<string, IList<IContextAwareDataTemplateProvider>> ByContext 
            = new Dictionary<string, IList<IContextAwareDataTemplateProvider>>();

        Cache InternalCache = new Cache();

        protected bool UseCache { get; set; } = true;

        /// <summary>
        /// Add selectors in logical order from Highest order to Lowest
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="context"></param>
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

            if (context == null)
                context = "";

            // Internal Cache is used to store last succesfull result.
            // Reason for that is that often same selector is called many times in a row (e.g. when in list view)
            
            if (UseCache && InternalCache.TrySelectTemplate(item, container, context, isTooltip, out dt))
                return dt;

            // check this context specific selectors first

            var providers_by_context = (IList<IContextAwareDataTemplateProvider>)null;

            if (ByContext.TryGetValue(context, out providers_by_context))
            {
                for (int i = 0; i < providers_by_context.Count; i++)
                {
                    var provider = providers_by_context[i];

                    if (provider.TrySelectTemplate(item, container, context, isTooltip, out dt))
                    {
                        if (UseCache)
                        {
                            InternalCache.Context = context;
                            InternalCache.DataTemplateProvider = provider;
                            InternalCache.Item = new WeakReference<object>(item);
                        }

                        return dt;
                    }
                }
            }

            // check '*' selectors
            if (ByContext.TryGetValue("*", out providers_by_context))
            {
                for (int i = 0; i < providers_by_context.Count; i++)
                {
                    if (providers_by_context[i].TrySelectTemplate(item, container, context, isTooltip, out dt))
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
