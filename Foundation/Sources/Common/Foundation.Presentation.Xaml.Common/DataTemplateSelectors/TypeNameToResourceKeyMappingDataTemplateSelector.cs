using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SquaredInfinity.Foundation.Extensions;
using System.Windows.Controls.Primitives;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors
{
    public class TypeNameToResourceKeyMappingDataTemplateSelector : ResourcesTraversingDataTemplateSelector
    {
        protected override bool TryFindDataTemplate(
            ResourceDictionary resources,
            FrameworkElement resourcesOwner,
            DependencyObject itemContainer,
            object item,
            Type itemType,
            out DataTemplate dataTemplate)
        {
            dataTemplate = resources[itemType.FullName] as DataTemplate;
            if (dataTemplate != null)
                return true;

            dataTemplate = resources[itemType.Name] as DataTemplate;
            if (dataTemplate != null)
                return true;

            return false;
        }
    }
}
