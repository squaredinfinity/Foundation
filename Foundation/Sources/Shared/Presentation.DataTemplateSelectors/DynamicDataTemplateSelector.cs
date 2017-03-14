using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SquaredInfinity.Presentation.DataTemplateSelectors
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Old DynamicDataTemplateSelector has been replaced with DynamicResourcesTraversingDataTemplateSelector.
    /// </remarks>
    public class DynamicDataTemplateSelector : DataTemplateSelector
    {
        readonly Func<object, DependencyObject, DataTemplate> GetDataTemplate;

        public DynamicDataTemplateSelector(Func<object, DependencyObject, DataTemplate> getDataTemplate)
        {
            this.GetDataTemplate = getDataTemplate;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return GetDataTemplate(item, container);
        }
    }
}
