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
    public class TypeNameToResourceKeyMappingDataTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item == null)
                return null;

            var itemType = item.GetType();

            var result = (DataTemplate)null;

            // first try to find in passed container

            var container_frameworkElement = container as FrameworkElement;
            if (container_frameworkElement != null && TryFindDataTemplate(container_frameworkElement.Resources, itemType, out result))
                    return result;

            // then try all visual parents

            var parent = container_frameworkElement;

            while ((parent = parent.FindVisualParent<FrameworkElement>()) != null)
            {
                if (TryFindDataTemplate(parent.Resources, itemType, out result))
                    return result;
            }

            // then try globally

            if (Application.Current != null && TryFindDataTemplate(Application.Current.Resources, itemType, out result))
                return result;             

            return null;
        }

        protected virtual bool TryFindDataTemplate(ResourceDictionary dict, Type type, out DataTemplate dataTemplate)
        {
            dataTemplate = dict[type.FullName] as DataTemplate;
            if (dataTemplate != null)
                return true;

            dataTemplate = dict[type.Name] as DataTemplate;
            if (dataTemplate != null)
                return true;

            if(type.Name.EndsWith("ViewModel"))
            {
                dataTemplate = dict[type.Name.Substring(0, type.Name.Length - "ViewModel".Length)] as DataTemplate;
                if (dataTemplate != null)
                    return true;
            }

            return false;
        }
    }
}
