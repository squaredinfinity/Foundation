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
    public class ForwardTextInput
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
            typeof(ForwardTextInput),
            new PropertyMetadata(null, OnTargetChanged));

        static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement source = d as UIElement;

            source.PreviewTextInput -= source_PreviewTextInput;
            source.PreviewTextInput += source_PreviewTextInput;
        }

        static void source_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var c = sender as Control;

            var target = GetTarget(c);

            var forwarded_textInput = new TextComposition(InputManager.Current, Keyboard.FocusedElement, e.Text);

            var new_e = new TextCompositionEventArgs(e.Device as KeyboardDevice, forwarded_textInput)
            {
                RoutedEvent = TextCompositionManager.TextInputEvent
            };

            target.RaiseEvent(new_e);
        }

        #endregion
    }
}
