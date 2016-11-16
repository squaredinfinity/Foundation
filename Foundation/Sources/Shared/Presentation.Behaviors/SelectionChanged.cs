using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public partial class SelectionChanged
    {
       #region Command

        public static void SetCommand(System.Windows.Controls.ItemsControl element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(System.Windows.Controls.ItemsControl element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(SelectionChanged),
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as UIElement;

            if (c == null)
                return;

            var tv = c as System.Windows.Controls.TreeView;
            if (tv != null)
            {
                if ((ICommand)e.NewValue != null)
                {
                    tv.SelectedItemChanged -= tv_SelectedItemChanged;
                    tv.SelectedItemChanged += tv_SelectedItemChanged;

                    tv.GotFocus -= OnGotFocus;
                    tv.GotFocus += OnGotFocus;
                }
                else
                {
                    tv.SelectedItemChanged -= tv_SelectedItemChanged;
                    tv.GotFocus -= OnGotFocus;
                }

                return;
            }

            var selector = c as System.Windows.Controls.Primitives.Selector;
            if(selector != null)
            {
                if ((ICommand)e.NewValue != null)
                {
                    selector.SelectionChanged -= selector_SelectionChanged;
                    selector.SelectionChanged += selector_SelectionChanged;

                    selector.GotFocus -= OnGotFocus;
                    selector.GotFocus += OnGotFocus;
                }
                else
                {
                    selector.SelectionChanged -= selector_SelectionChanged;
                    selector.GotFocus -= OnGotFocus;
                }

                return;
            }
        }

        static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var ic = sender as System.Windows.Controls.ItemsControl;
            if (ic == null)
                return;

            var shouldContinue = GetReevaluateOnFocusChanged(ic);

            if (!shouldContinue)
                return;

            var tv = sender as System.Windows.Controls.TreeView;
            if (tv != null)
            {
                var command = GetCommand(tv);

                if (command != null && command.CanExecute(tv.SelectedItem))
                {
                    command.Execute(tv.SelectedItem);
                }

                return;
            }

            var selector = sender as System.Windows.Controls.Primitives.Selector;
            if(selector != null)
            {
                var command = GetCommand(selector);

                if (command != null && command.CanExecute(selector.SelectedItem))
                {
                    command.Execute(selector.SelectedItem);
                }

                return;
            }
        }

        static void selector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selector = sender as System.Windows.Controls.Primitives.Selector;

            if (selector == null)
                return;

            var command = GetCommand(selector);

            if (command != null && command.CanExecute(selector.SelectedItem))
            {
                command.Execute(selector.SelectedItem);
            }
        }

        static void tv_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tv = sender as System.Windows.Controls.TreeView;

            if (tv == null)
                return;

            var command = GetCommand(tv);

            if (command != null && command.CanExecute(tv.SelectedItem))
            {
                command.Execute(tv.SelectedItem);
            }
        }


        #endregion

        #region ReevaluateOnFocusChanged

        public static void SetReevaluateOnFocusChanged(System.Windows.Controls.ItemsControl element, bool value)
        {
            element.SetValue(ReevaluateOnFocusChangedProperty, value);
        }

        public static bool GetReevaluateOnFocusChanged(System.Windows.Controls.ItemsControl element)
        {
            return (bool)element.GetValue(ReevaluateOnFocusChangedProperty);
        }

        public static readonly DependencyProperty ReevaluateOnFocusChangedProperty =
            DependencyProperty.RegisterAttached(
            "ReevaluateOnFocusChanged",
            typeof(bool),
            typeof(SelectionChanged),
            new PropertyMetadata(true));

        #endregion

    }
}
