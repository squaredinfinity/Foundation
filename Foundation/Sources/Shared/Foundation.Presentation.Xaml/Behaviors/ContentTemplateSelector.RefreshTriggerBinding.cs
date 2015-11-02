using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public class ContentTemplateSelector
    {
        #region RefreshTriggerBinding

        public static void SetRefreshTriggerBinding(ContentPresenter element, bool value)
        {
            element.SetValue(RefreshTriggerBindingProperty, value);
        }

        public static object GetRefreshTriggerBinding(ContentPresenter element)
        {
            return (object)element.GetValue(RefreshTriggerBindingProperty);
        }

        public static readonly DependencyProperty RefreshTriggerBindingProperty =
            DependencyProperty.RegisterAttached(
            "RefreshTriggerBinding",
            typeof(object),
            typeof(ContentTemplateSelector),
            new PropertyMetadata(null, OnRefreshTriggerBindingChanged));

        static void OnRefreshTriggerBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ContentPresenter;

            if (c == null)
                return;

            var oldSelector = c.ContentTemplateSelector;

            c.ContentTemplateSelector = null;

            c.ContentTemplateSelector = oldSelector;
        }


        #endregion
    }
}
