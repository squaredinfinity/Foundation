using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors
{
    public abstract class ResourcesTraversingDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return null;

            var itemType = item.GetType();

            var result = (DataTemplate)null;

            // first try to find in passed container

            var container_frameworkElement = container as FrameworkElement;
            if (container_frameworkElement != null
                && TryFindDataTemplate(container_frameworkElement.Resources, container_frameworkElement, container, item, itemType, out result))
            {
                return result;
            }

            // then try all visual parents

            var parent = (DependencyObject) container_frameworkElement;

            // navigate up visual (first) and logical tree (second) to find the resource
            // logical jump is required in cases where child elements are rendered in a popup (e.g. in ComboBox)
            // otherwise visual tree would end at the popup root and resources from combobox itself would never be checked

            while ((parent = parent.GetVisualOrLogicalParent()) != null)
            {
                var parent_frameworkElement = parent as FrameworkElement;

                if (parent_frameworkElement == null)
                    continue;

                if (TryFindDataTemplate(parent_frameworkElement.Resources, parent_frameworkElement, container, item, itemType, out result))
                    return result;
            }

            // then try globally

            if (Application.Current != null
                && TryFindDataTemplate(Application.Current.Resources, null, container, item, itemType, out result))
            {
                return result;
            }

            return null;
        }

        protected abstract bool TryFindDataTemplate(
            ResourceDictionary resources, 
            FrameworkElement resourcesOwner,
            DependencyObject itemContainer,
            object item, 
            Type itemType,
            out DataTemplate dataTemplate);
    }
}
