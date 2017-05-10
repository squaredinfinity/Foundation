using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SquaredInfinity.Windows.Behaviors
{
    public class DoubleClick
    {
        #region CommandParameters

        public static void SetCommandParameter(UIElement element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(UIElement element)
        {
            return (object)element.GetValue(CommandParameterProperty);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(DoubleClick),
            new PropertyMetadata(null));

        #endregion

        #region Command

        public static void SetCommand(UIElement element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command", 
            typeof(ICommand),
            typeof(DoubleClick), 
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as UIElement;

            if (c == null)
                return;

            if((ICommand)e.NewValue != null)
            {
                c.MouseUp += C_MouseUp;
            }
            else
            {
                c.MouseUp -= C_MouseUp;
            }
        }

        private static void C_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var c = sender as UIElement;

            if (c == null)
                return;

            // at least two clicks
            if (e.ClickCount < 2)
                return;

            // click only captured on left button
            // todo: this may need changing depending on mouse settings (left/right handed)
            if (e.ChangedButton != MouseButton.Left || e.ButtonState != MouseButtonState.Released)
                return;

            var command = GetCommand(c);

            var parameter = GetCommandParameter(c);

            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }

            e.Handled = true;
        }

        #endregion
    }
}
