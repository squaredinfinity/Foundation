using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public class ParentContentPresenter
    {
        #region Margin

        public static void SetMargin(Control element, Thickness? value)
        {
            element.SetValue(MarginProperty, value);
        }

        public static Thickness? GetMargin(Control element)
        {
            return (Thickness)element.GetValue(MarginProperty);
        }

        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.RegisterAttached(
            "Margin",
            typeof(Thickness?),
            typeof(ParentContentPresenter),
            new PropertyMetadata(null, OnMarginChanged));

        static void OnMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement source = d as UIElement;

            if (e.NewValue == null)
                return;

            var cp = source.FindVisualParent<ContentPresenter>();

            if (cp != null)
                cp.SetValue(FrameworkElement.MarginProperty, e.NewValue);
        }

        #endregion
    }
}
