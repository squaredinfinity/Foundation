using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SquaredInfinity.Presentation.Behaviors
{
    public class MouseMove
    {
        #region CommandParameters

        public static void SetCommandParameter(FrameworkElement element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(FrameworkElement element)
        {
            return (object)element.GetValue(CommandParameterProperty);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(MouseMove),
            new PropertyMetadata(null));

        #endregion

        #region Command

        public static void SetCommand(FrameworkElement element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(FrameworkElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(MouseMove),
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = d as FrameworkElement;

            if (fe == null)
                return;

            if ((ICommand)e.NewValue != null)
            {
                fe.MouseMove += fe_MouseMove;
            }
            else
            {
                fe.MouseMove -= fe_MouseMove;
            }
        }

        static void fe_MouseMove(object sender, MouseEventArgs e)
        {
            var fe = sender as FrameworkElement;

            if (fe == null)
                return;

            var command = GetCommand(fe);

            var parameter = GetCommandParameter(fe);

            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }

        #endregion
    }
}
