using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public class ItemsSourceWithPreserveSelection
    {
        #region ItemsSource

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(ItemsSourceWithPreserveSelection),
            new PropertyMetadata(null, OnItemsSourceChanged));

        public static void SetItemsSource(Selector element, IEnumerable value)
        {
            element.SetValue(ItemsSourceProperty, value);
        }

        public static IEnumerable GetItemsSource(Selector element)
        {
            return (IEnumerable)element.GetValue(ItemsSourceProperty);
        }

        static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sel = d as Selector;

            if (sel == null)
                return;

            //# preserve original SelectedValue binding
            
            var selectedValeBinding = BindingOperations.GetBinding(sel, Selector.SelectedValueProperty);

            //# clear SelectedValue binding (so that underlying bound property won't be wrongly updated when ItemsSource changes)
            sel.ClearBindingOrReplaceWithDummyValue(Selector.SelectedValueProperty);

            try
            {
                sel.ItemsSource = e.NewValue as IEnumerable;
            }
            finally
            {
                //# restore original SelectedValue binding

                sel.SelectedItem = null;

                if (selectedValeBinding != null)
                    BindingOperations.SetBinding(sel, Selector.SelectedValueProperty, selectedValeBinding);
            }
        }

        #endregion
    }
}
