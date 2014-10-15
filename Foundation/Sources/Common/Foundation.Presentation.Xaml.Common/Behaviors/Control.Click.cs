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

    public class Click
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
            typeof(Click),
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
            typeof(Click), 
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as UIElement;

            if (c == null)
                return;

            if((ICommand)e.NewValue != null)
            {
                c.PreviewMouseUp += c_PreviewMouseUp;
            }
            else
            {
                c.PreviewMouseUp -= c_PreviewMouseUp;
            }
        }

        static void c_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var c = sender as UIElement;

            if (c == null)
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
