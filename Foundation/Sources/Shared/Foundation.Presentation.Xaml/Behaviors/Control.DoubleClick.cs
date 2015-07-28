using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public class DoubleClick
    {
        #region CommandParameters

        public static void SetCommandParameter(Control element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(Control element)
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

        public static void SetCommand(Control element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(Control element)
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
            var c = d as Control;

            if (c == null)
                return;

            if((ICommand)e.NewValue != null)
            {
                c.MouseDoubleClick += c_MouseDoubleClick;
            }
            else
            {
                c.MouseDoubleClick -= c_MouseDoubleClick;
            }
        }

        static void c_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var c = sender as Control;

            if(c == null)
                return;

            var command = GetCommand(c);

            var parameter = GetCommandParameter(c);

            if (command != null && command.CanExecute(parameter))
                command.Execute(parameter);
        }

        #endregion
    }
}
