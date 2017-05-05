using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SquaredInfinity.Extensions
{
    public static class ItemsControlExtensions
    {
        static readonly PropertyInfo PROPERTY_CanSelectMultipleItems;

        static ItemsControlExtensions()
        {
            PROPERTY_CanSelectMultipleItems =
                typeof(MultiSelector)
                .GetProperty("CanSelectMultipleItems",
                BindingFlags.Instance | BindingFlags.NonPublic);
        }
        public static IEnumerable GetSelectedItems(this ItemsControl itemsControl)
        {
            var multiSelector = itemsControl as MultiSelector;
            if (multiSelector != null)
            {
                return multiSelector.SelectedItems;
            }

            var listBox = itemsControl as ListBox;
            if (listBox != null)
            {
                if (listBox.SelectionMode == SelectionMode.Single)
                {
                    return Enumerable.Repeat(listBox.SelectedItem, 1);
                }
                else
                {
                    return listBox.SelectedItems;
                }
            }

            var treeView = itemsControl as TreeView;
            if (treeView != null)
            {
                return Enumerable.Repeat(treeView.SelectedItem, 1);
            }

            var selector = itemsControl as Selector;
            if (selector != null)
            {
                return Enumerable.Repeat(selector.SelectedItem, 1);
            }

            return Enumerable.Empty<object>();
        }

        public static bool IsItemSelected(this ItemsControl itemsControl, object item)
        {
            var multiSelector = itemsControl as MultiSelector;
            if (multiSelector != null)
            {
                return multiSelector.SelectedItems.Contains(item);
            }

            var listBox = itemsControl as ListBox;
            if (listBox != null)
            {
                return listBox.SelectedItems.Contains(item);
            }

            var treeView = itemsControl as TreeView;
            if (treeView != null)
            {
                return treeView.SelectedItem == item;
            }

            var selector = itemsControl as Selector;
            if (selector != null)
            {
                return selector.SelectedItem == item;
            }

            return false;
        }

        public static bool CanSelectMultipleItems(this ItemsControl itemsControl)
        {
            if (itemsControl is MultiSelector)
                return (bool)PROPERTY_CanSelectMultipleItems.GetValue(itemsControl, null);

            var listBox = itemsControl as ListBox;
            if (listBox != null)
            {
                return listBox.SelectionMode != SelectionMode.Single;
            }

            return false;
        }
    }
}
