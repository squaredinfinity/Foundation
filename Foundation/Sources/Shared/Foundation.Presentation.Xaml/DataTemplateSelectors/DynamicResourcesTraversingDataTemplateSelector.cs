using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors
{
    public class DynamicResourcesTraversingDataTemplateSelector : ResourcesTraversingDataTemplateSelector
    {
        readonly Func<object, DependencyObject, string> GetTemplateResourceKey;

        public DynamicResourcesTraversingDataTemplateSelector(Func<object, DependencyObject, string> getTemplateResourceKey)
        {
            this.GetTemplateResourceKey = getTemplateResourceKey;
        }

        protected override bool TryFindDataTemplate(
            ResourceDictionary resources,
            FrameworkElement resourcesOwner,
            DependencyObject itemContainer,
            object item,
            Type itemType,
            out DataTemplate dataTemplate)
        {
            var key = GetTemplateResourceKey(item, itemContainer);

            if (key == null)
            {
                dataTemplate = null;
                return false;
            }

            dataTemplate = resources[key] as DataTemplate;
            if (dataTemplate != null)
                return true;

            return false;
        }
    }
}
