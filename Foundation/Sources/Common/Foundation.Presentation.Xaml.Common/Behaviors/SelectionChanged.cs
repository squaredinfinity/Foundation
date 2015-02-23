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

        public static void SetCommand(System.Windows.Controls.TreeView element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(System.Windows.Controls.TreeView element)
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
                }
                else
                {
                    tv.SelectedItemChanged -= tv_SelectedItemChanged;
                }
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
    }
}
