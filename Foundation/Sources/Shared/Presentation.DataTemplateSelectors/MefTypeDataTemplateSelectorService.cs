using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Presentation.DataTemplateSelectors
{
    //public class MefTypeDataTemplateSelectorService : IContextAwareDataTemplateSelectorService
    //{
    //    [ImportMany]
    //    List<IContextAwareDataTemplateSelector> Selectors;

    //    public void RegisterDataTemplateSelector(IContextAwareDataTemplateSelector selector)
    //    {

    //    }

    //    public DataTemplate SelectTemplate(object item, DependencyObject container, string context, bool isTooltip)
    //    {
    //        if (Selectors == null)
    //        {
    //            var compositionService = ServiceLocator.Current.GetInstance<ICompositionService>();
    //            compositionService.SatisfyImportsOnce(this);
    //        }

    //        var dt = (DataTemplate)null;

    //        for (int i = 0; i < Selectors.Count; i++)
    //        {
    //            var selector = Selectors[i];

    //            if (selector.TrySelectTemplate(item, container, context, isTooltip, out dt))
    //            {
    //                return dt;
    //            }
    //        }

    //        return null;
    //    }
    //}
}
