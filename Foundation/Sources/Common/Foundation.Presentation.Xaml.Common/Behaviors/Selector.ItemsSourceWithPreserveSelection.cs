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



        #region SelectedValueBinding_RestoreAfterInitialization

        public static readonly DependencyProperty SelectedValueBinding_RestoreAfterInitializationProperty =
            DependencyProperty.RegisterAttached(
            "SelectedValueBinding_RestoreAfterInitialization",
            typeof(BindingBase),
            typeof(ItemsSourceWithPreserveSelection));

        public static void SetSelectedValueBinding_RestoreAfterInitialization(Selector element, BindingBase value)
        {
            element.SetValue(SelectedValueBinding_RestoreAfterInitializationProperty, value);
        }

        public static BindingBase GetSelectedValueBinding_RestoreAfterInitialization(Selector element)
        {
            return (BindingBase)element.GetValue(SelectedValueBinding_RestoreAfterInitializationProperty);
        }

        #endregion

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
            
            var selectedValeBinding = BindingOperations.GetBindingBase(sel, Selector.SelectedValueProperty);

            //# clear SelectedValue binding (so that underlying bound property won't be wrongly updated when ItemsSource changes)
            sel.ClearBindingOrReplaceWithDummyValue(Selector.SelectedValueProperty);

            try
            {
                sel.ItemsSource = e.NewValue as IEnumerable;

                if (!sel.IsInitialized)
                {
                    sel.Initialized += sel_Initialized;

                    SetSelectedValueBinding_RestoreAfterInitialization(sel, selectedValeBinding);
                }
            }
            finally
            {
                //# restore original SelectedValue binding

                //sel.SelectedItem = null;

                if (sel.IsInitialized && selectedValeBinding != null)
                    BindingOperations.SetBinding(sel, Selector.SelectedValueProperty, selectedValeBinding);
            }
        }

        static void sel_Initialized(object sender, EventArgs e)
        {
            var sel = sender as Selector;

            sel.Initialized -= sel_Initialized;

            var selectedValeBinding = GetSelectedValueBinding_RestoreAfterInitialization(sel);

            if (selectedValeBinding != null)
            {
                BindingOperations.SetBinding(sel, Selector.SelectedValueProperty, selectedValeBinding);
            }
        }

        #endregion
    }
}
