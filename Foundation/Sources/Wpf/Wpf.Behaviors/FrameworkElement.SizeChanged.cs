using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public class SizeChanged
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
            typeof(SizeChanged),
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
            typeof(SizeChanged),
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = d as FrameworkElement;

            if (fe == null)
                return;

            if ((ICommand)e.NewValue != null)
            {
                fe.SizeChanged += fe_SizeChanged;
            }
            else
            {
                fe.SizeChanged -= fe_SizeChanged;
            }
        }

        static void fe_SizeChanged(object sender, SizeChangedEventArgs e)
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
