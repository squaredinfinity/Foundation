using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public class ItemDoubleClick
    {
        #region Command

        public static void SetCommand(Selector element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(Selector element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command", 
            typeof(ICommand),
            typeof(ItemDoubleClick), 
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = d as Selector;

            if (selector == null)
                return;

            if((ICommand)e.NewValue != null)
            {
                selector.MouseDoubleClick += c_MouseDoubleClick;
            }
            else
            {
                selector.MouseDoubleClick -= c_MouseDoubleClick;
            }
        }

        static void c_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selector = sender as Selector;

            if(selector == null)
                return;

            // check if double click comes from [child] item, of from selector area (e.g. scrollbar)
            var sourceFe = e.OriginalSource as FrameworkElement;

            var lbi = sourceFe.FindVisualParent<ListBoxItem>(stopSearchAt: selector);

            if (lbi == null)
                return;

            var command = GetCommand(selector);

            if (command == null)
                return;
            
            if(command.CanExecute(null))
                command.Execute(null);
        }

        #endregion
    }
}
