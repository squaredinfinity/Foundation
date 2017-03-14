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

namespace SquaredInfinity.Presentation.Behaviors
{
    public class MultiSelection
    {
        #region Selected Items

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
            "SelectedItems",
            typeof(IList),
            typeof(MultiSelection),
            new PropertyMetadata(null, OnMultiSelectionChanged));

        public static void SetSelectedItems(Selector element, IList value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        public static IList GetSelectedItems(Selector element)
        {
            return (IList)element.GetValue(SelectedItemsProperty);
        }

        static void OnMultiSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sel = d as Selector;

            sel.SelectionChanged -= sel_SelectionChanged;
            sel.SelectionChanged += sel_SelectionChanged;
        }

        static void sel_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var sel = sender as Selector;
            
            var lv = sel as ListView;

            if (lv != null)
            {
                // if SelectedItems is bound to INotifyCollectionChanged, then update the collection
                //var ncc = GetSelectedItems(lv) as INotifyCollectionChanged;

                //if(ncc != null)
                //{
                //    //var ocx = ncc as IBulkUpdatesCollection

                    
                //}

                var boundCollection = GetSelectedItems(lv);

                var boundList = boundCollection as IList;

                if(boundList != null)
                {
                    boundList.Clear();
                    
                    foreach(var item in lv.SelectedItems)
                    {
                        boundList.Add(item);
                    }
                }



                //SetSelectedItems(lv, lv.SelectedItems);
            }

        }

        #endregion
    }
}
