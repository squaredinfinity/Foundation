using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SquaredInfinity.Presentation.Behaviors
{
    public class ForwardKeyEvents
    {
        #region Target

        public static void SetTarget(Control element, UIElement value)
        {
            element.SetValue(TargetProperty, value);
        }

        public static UIElement GetTarget(Control element)
        {
            return (UIElement)element.GetValue(TargetProperty);
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.RegisterAttached(
            "Target",
            typeof(UIElement),
            typeof(ForwardKeyEvents),
            new PropertyMetadata(null, OnTargetChanged));

        static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement source = d as UIElement;

            source.PreviewKeyDown -= source_PreviewKeyDown;   
            source.PreviewKeyDown += source_PreviewKeyDown;

            source.KeyDown -= source_KeyDown;
            source.KeyDown += source_KeyDown;

            source.PreviewKeyUp -= source_PreviewKeyUp;
            source.PreviewKeyUp += source_PreviewKeyUp;

            source.KeyUp -= source_KeyUp;
            source.KeyUp += source_KeyUp;
        }

        static void source_KeyUp(object sender, KeyEventArgs e)
        {
            CreateAndRaiseNewEventArgs(sender, e);
        }

        static void source_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            CreateAndRaiseNewEventArgs(sender, e);
        }

        static void source_KeyDown(object sender, KeyEventArgs e)
        {
            CreateAndRaiseNewEventArgs(sender, e);
        }

        static void source_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            CreateAndRaiseNewEventArgs(sender, e);
        }

        static void CreateAndRaiseNewEventArgs(object sender, KeyEventArgs e)
        {
            var c = sender as Control;

            var target = GetTarget(c);

            var new_e = new KeyEventArgs(e.Device as KeyboardDevice, PresentationSource.FromVisual(target), e.Timestamp, e.Key)
            {
                RoutedEvent = e.RoutedEvent
            };

            target.RaiseEvent(new_e);

            if (new_e.Handled)
                e.Handled = true;
        }

        #endregion
    }
}
