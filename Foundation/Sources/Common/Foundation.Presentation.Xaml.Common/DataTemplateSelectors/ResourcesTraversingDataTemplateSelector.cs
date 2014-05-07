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

            var parent = container_frameworkElement;

            while ((parent = parent.FindVisualParent<FrameworkElement>()) != null)
            {
                if (TryFindDataTemplate(parent.Resources, parent, container, item, itemType, out result))
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
