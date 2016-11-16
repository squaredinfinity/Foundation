using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WPFControls = System.Windows.Controls;
using System.Windows.Controls;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public partial class TreeView
    {
        #region AllowMultiSelection

        public static readonly DependencyProperty AllowMultiSelectionProperty =
            DependencyProperty.RegisterAttached(
            "AllowMultiSelection",
            typeof(bool),
            typeof(TreeView),
            new PropertyMetadata(false, OnAllowMultiSelectionChanged));

        public static void SetAllowMultiSelection(WPFControls.TreeView element, bool value)
        {
            element.SetValue(AllowMultiSelectionProperty, value);
        }

        public static bool GetAllowMultiSelection(WPFControls.TreeView element)
        {
            return (bool)element.GetValue(AllowMultiSelectionProperty);
        }
        
        #endregion

        #region SelectedItems

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
            "SelectedItems",
            typeof(List<WPFControls.TreeViewItem>),
            typeof(TreeView),
            new PropertyMetadata(new List<WPFControls.TreeViewItem>()));

        public static void SetSelectedItems(WPFControls.TreeView element, List<WPFControls.TreeViewItem> value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        public static List<WPFControls.TreeViewItem> GetSelectedItems(WPFControls.TreeView element)
        {
            return (List<WPFControls.TreeViewItem>)element.GetValue(SelectedItemsProperty);
        }

        #endregion

        #region InitiallySelectedItem

        public static readonly DependencyProperty InitiallySelectedItem =
            DependencyProperty.RegisterAttached(
            "InitiallySelectedItem",
            typeof(WPFControls.TreeViewItem),
            typeof(TreeView),
            new PropertyMetadata(null));

        public static void SetInitiallySelectedItem(WPFControls.TreeView element, WPFControls.TreeViewItem value)
        {
            element.SetValue(InitiallySelectedItem, value);
        }

        public static WPFControls.TreeViewItem GetInitiallySelectedItem(WPFControls.TreeView element)
        {
            return (WPFControls.TreeViewItem) element.GetValue(InitiallySelectedItem);
        }

        #endregion

        private static void OnAllowMultiSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (IsSelectionChangeActiveProperty == null)
                return;

            var tv = d as WPFControls.TreeView;
            if (tv == null)
                return;

            if ((bool)e.NewValue == true)
            {
                tv.SelectedItemChanged -= MultiSelection_TreeView_SelectedItemChanged;
                tv.SelectedItemChanged += MultiSelection_TreeView_SelectedItemChanged;

                tv.PreviewMouseDown += tv_PreviewMouseDown;
                tv.PreviewGotKeyboardFocus += tv_PreviewGotKeyboardFocus;
            }
            else
            {
                tv.SelectedItemChanged -= MultiSelection_TreeView_SelectedItemChanged;
            }
        }

        static void tv_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var tv = sender as WPFControls.TreeView;
            if (tv == null)
                return;

            var tvi = (e.OriginalSource as DependencyObject).FindVisualParent<TreeViewItem>();

            if (tvi == null)
                return;

            bool isEventHandled = false;

            HandlePreviewSelectionEvent(tv, tvi, out isEventHandled);

            e.Handled = isEventHandled;
        }

        static void tv_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var tv = sender as WPFControls.TreeView;
            if (tv == null)
                return;

            var tvi = (e.OriginalSource as DependencyObject).FindVisualParent<TreeViewItem>();

            if (tvi == null)
                return;


            bool isEventHandled = false;

            HandlePreviewSelectionEvent(tv, tvi, out isEventHandled);

            e.Handled = isEventHandled;
        }

        static void HandlePreviewSelectionEvent(WPFControls.TreeView tv, TreeViewItem tvi, out bool isEventHandled)
        {
            isEventHandled = false;

            if (tvi.IsSelected)
            {
                //# Item has been previously selected (we're in preview event and it is already selected)

                var allowMultiSelection = GetAllowMultiSelection(tv);

                var selectedItems = GetSelectedItems(tv);

                if (allowMultiSelection)
                {
                    if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    {
                        //# Ctrl pressed and item has been previously selected (IsSelected == true) => Unselect this item

                        selectedItems.Remove(tvi);

                        tvi.IsSelected = false;

                        isEventHandled = true;

                        return;
                    }
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        //# Shift pressed - block selection
                        //# Item has been previously selected (IsSelected == true) => update selection range

                        int blockSelectionStartIndex = -1;
                        int blockSelectionEndIndex = -1;
                        WPFControls.TreeViewItem parentTvi = null;

                        if (!TryGetNewBlockSelectionDetails(tv, selectedItems, tvi, out blockSelectionStartIndex, out blockSelectionEndIndex, out parentTvi))
                        {
                            //# this is not a valid block selection, clear previous selection data
                            selectedItems.Clear();
                            selectedItems.Add(tvi);

                            isEventHandled = false;
                            return;
                        }

                        using (new SelectionChangedInhibitor(tv))
                        {
                            var removedItems =
                                (from si in selectedItems
                                 let index = parentTvi.ItemContainerGenerator.IndexFromContainer(si)
                                 where index < blockSelectionStartIndex || index > blockSelectionEndIndex
                                 select si)
                                 .ToArray();

                            foreach (var i in removedItems)
                            {
                                i.IsSelected = false;
                                selectedItems.Remove(i);
                            }

                            for (var i = blockSelectionStartIndex; i <= blockSelectionEndIndex; i++)
                            {
                                //# mark all selected items as selected
                                var tvi2 = (parentTvi.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem);
                                tvi2.IsSelected = true;

                                selectedItems.Add(tvi2);
                            }
                        }

                        isEventHandled = true;
                        return;
                    }
                }

                if (!allowMultiSelection || !isEventHandled)
                {
                    if (selectedItems.Count > 0)
                    {
                        // make new selected item the only selection

                        using (new SelectionChangedInhibitor(tv))
                        {
                            foreach (var i in selectedItems)
                                i.IsSelected = false;
                        }

                        selectedItems.Clear();

                        selectedItems.Add(tvi);

                        SetInitiallySelectedItem(tv, tvi);

                        tvi.IsSelected = true;
                    }
                }
            }
            else
            {
                // noting to do here for now
            }
        }

        static void MultiSelection_TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tv = sender as WPFControls.TreeView;
            if (tv == null)
                return;

            var tvi = SelectedContainerProperty.GetValue(tv, null) as WPFControls.TreeViewItem;
            if (tvi == null)
            {
                return;
            }

            var selectedItems = GetSelectedItems(tv);

            var allowMultiSelection = GetAllowMultiSelection(tv);

            var isSelectionHandled = false;

            if (allowMultiSelection)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    //# mark all selected items as selected
                    using (new SelectionChangedInhibitor(tv))
                    {
                        foreach (var i in selectedItems.Except(new WPFControls.TreeViewItem[] { tvi }))
                        {
                            i.IsSelected = true;
                        }
                    }

                    if (selectedItems.Contains(tvi))
                    {
                        tvi.IsSelected = false;
                        selectedItems.Remove(tvi);
                    }
                    else
                    {
                        selectedItems.Add(tvi);
                    }

                    isSelectionHandled = true;
                }
                else if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                {
                    int blockSelectionStartIndex = -1;
                    int blockSelectionEndIndex = -1;
                    WPFControls.TreeViewItem parentTvi = null;

                    if (!TryGetNewBlockSelectionDetails(tv, selectedItems, tvi, out blockSelectionStartIndex, out blockSelectionEndIndex, out parentTvi))
                    {
                        //# this is not a valid block selection, clear previous selection data
                        selectedItems.Clear();
                        selectedItems.Add(tvi);

                        return;
                    }

                    using (new SelectionChangedInhibitor(tv))
                    {
                        //# remove items which are no longer selected
                        var itemsToRemove =
                                    (from si in selectedItems
                                     let index = parentTvi.ItemContainerGenerator.IndexFromContainer(si)
                                     where index < blockSelectionStartIndex || index > blockSelectionEndIndex
                                     select si)
                                    .ToArray();

                        foreach (var i in itemsToRemove)
                        {
                            i.IsSelected = false;
                            selectedItems.Remove(i);
                        }
                        
                        for (var i = blockSelectionStartIndex; i <= blockSelectionEndIndex; i++)
                        {
                            //# mark all selected items as selected
                            var tvi2 = (parentTvi.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItem);
                            tvi2.IsSelected = true;

                            selectedItems.Add(tvi2);
                        }
                    }

                    isSelectionHandled = true;
                }
            }
            
            if (!allowMultiSelection || !isSelectionHandled)
            {
                //# single selection
                //# unselect all previously selected
                foreach (var i in selectedItems.Except(new WPFControls.TreeViewItem[] { tvi }))
                {
                    i.IsSelected = false;
                }

                selectedItems.Clear();

                selectedItems.Add(tvi);

                SetInitiallySelectedItem(tv, tvi);
            }
        }

        private static bool TryGetNewBlockSelectionDetails(
            WPFControls.TreeView treeView,
            IEnumerable<WPFControls.TreeViewItem> alreadySelectedItems,
            WPFControls.TreeViewItem newlySelectedItem,
            out int blockSelectionStartIndex,
            out int blockSelectionEndIndex,
            out WPFControls.TreeViewItem parentTreeViewItem)
        {
            blockSelectionStartIndex = -1;
            blockSelectionEndIndex = -1;

            var parentTvi = newlySelectedItem.FindVisualParent<TreeViewItem>();

            parentTreeViewItem = parentTvi;

            if (parentTvi == null)
                return false;

            var hasSelectedItemsFromDifferentParent =
                (from i in alreadySelectedItems
                    where i.FindVisualParent<TreeViewItem>() != parentTvi
                    select i).Any();

            if (!hasSelectedItemsFromDifferentParent)
            {
                var initiallySelectedItem = GetInitiallySelectedItem(treeView);

                // shift pressed but this is a first item selected, so there's no initally selected item yet
                if (initiallySelectedItem == null)
                {
                    return false;
                }

                var initiallySelectedIndex =
                    parentTvi.ItemContainerGenerator.IndexFromContainer(initiallySelectedItem);

                var newSelectedIndex = parentTvi.ItemContainerGenerator.IndexFromContainer(newlySelectedItem);

                if (initiallySelectedIndex == -1 || newSelectedIndex == -1)
                    return false;

                blockSelectionStartIndex = Math.Min(initiallySelectedIndex, newSelectedIndex);
                blockSelectionEndIndex = Math.Max(initiallySelectedIndex, newSelectedIndex);

                return true;
            }

            return false;
        }
    }
}
