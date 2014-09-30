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
            typeof(Click), 
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as Control;

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
            var c = sender as Control;

            if (c == null)
                return;

            var command = GetCommand(c);

            if (command != null && command.CanExecute(null))
            {
                command.Execute(null);
            }

            e.Handled = true;
        }

        #endregion
    }
}
