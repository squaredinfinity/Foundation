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
                for (int i = 0; i < Selectors.Count; i++)
                {
                    var selector = Selectors[i];

                    if (selector.TrySelectTemplate(item, container, context, isTooltip, out dt))
                    {
                        return dt;
                    }
                }
            }

            return null;
        }
    }
}
