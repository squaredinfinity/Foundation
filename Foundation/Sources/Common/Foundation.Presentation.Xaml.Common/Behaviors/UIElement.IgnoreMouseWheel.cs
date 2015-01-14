using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace  xxx.Foundation.Presentation.Behaviors
{
    public static partial class UIElementBehaviors
    {
        #region IgnoreMouseWheel

        public static readonly DependencyProperty IgnoreMouseWheelProperty =
            DependencyProperty.RegisterAttached(
            "IgnoreMouseWheel",
            typeof(bool),
            typeof(UIElementBehaviors),
            new PropertyMetadata(false, OnIgnoreMouseWheelPropertyChanged));

        public static void SetIgnoreMouseWheel(System.Windows.UIElement element, bool value)
        {
            element.SetValue(IgnoreMouseWheelProperty, value);
        }

        public static bool GetIgnoreMouseWheel(System.Windows.UIElement element)
        {
            return (bool)element.GetValue(IgnoreMouseWheelProperty);
        }

        #endregion

        private static void OnIgnoreMouseWheelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (IgnoreMouseWheelProperty == null)
                return;

            var uiElement = d as System.Windows.UIElement;
            if (uiElement == null)
                return;

            if ((bool)e.NewValue == true)
            {
                uiElement.PreviewMouseWheel -= IgnoreMouseWheel_PreviewMouseWheel;
                uiElement.PreviewMouseWheel += IgnoreMouseWheel_PreviewMouseWheel;
            }
            else
            {
                uiElement.PreviewMouseWheel -= IgnoreMouseWheel_PreviewMouseWheel;
            }
        }

        static void IgnoreMouseWheel_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;

            var uiElement = sender as System.Windows.UIElement;

            if (uiElement == null)
                return;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = System.Windows.UIElement.MouseWheelEvent;

            uiElement.RaiseEvent(e2);            
        }

    }
}
