using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection;
using System.Collections;
using System.Windows.Controls.Primitives;
using SquaredInfinity.Extensions;
using System.ComponentModel;

namespace SquaredInfinity.Presentation.DragDrop.Utilities
{
  public static class ItemsControlExtensions
  {
    
      /// <summary>
      /// Returns *raw underlying collection* behind specified Items Control.
      /// If Items Control is bound to a Collection View, it will return the collection used by Collection View
      /// </summary>
      /// <param name="itemsControl"></param>
      /// <returns></returns>
      public static IEnumerable GetRawUnderlyingCollection(this ItemsControl itemsControl)
      {
          if(itemsControl.ItemsSource != null)
          {
              return GetRawUnderlyingCollection(itemsControl.ItemsSource);
          }

          return GetRawUnderlyingCollection(itemsControl.Items.SourceCollection);
      }

      static IEnumerable GetRawUnderlyingCollection(IEnumerable collection)
      {
          if (collection == null)
              return collection;

          var cv = collection as ICollectionView;
          if (cv != null)
              return GetRawUnderlyingCollection(cv.SourceCollection);

          return collection;
      }

    public static UIElement GetItemContainer(this ItemsControl itemsControl, UIElement child)
    {
      bool isItemContainer;
      var itemType = GetItemContainerType(itemsControl, out isItemContainer);

      if (itemType != null) {
        return isItemContainer
                 ? (UIElement)child.FindVisualParent(itemType, itemsControl)
                 : (UIElement)child.FindVisualParent(itemType);
      }

      return null;
    }

    public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position)
    {
      var inputElement = itemsControl.InputHitTest(position);
      var uiElement = inputElement as UIElement;

      if (uiElement != null) {
        return GetItemContainer(itemsControl, uiElement);
      }

      return null;
    }

    public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position,
                                               Orientation searchDirection)
    {
      bool isItemContainer;
      var itemContainerType = GetItemContainerType(itemsControl, out isItemContainer);

      Geometry hitTestGeometry;

      if (typeof(TreeViewItem).IsAssignableFrom(itemContainerType)) {
        hitTestGeometry = new LineGeometry(new Point(0, position.Y), new Point(itemsControl.RenderSize.Width, position.Y));
      } else {
        switch (searchDirection) {
          case Orientation.Horizontal:
            hitTestGeometry = new LineGeometry(new Point(0, position.Y), new Point(itemsControl.RenderSize.Width, position.Y));
            break;
          case Orientation.Vertical:
            hitTestGeometry = new LineGeometry(new Point(position.X, 0), new Point(position.X, itemsControl.RenderSize.Height));
            break;
          default:
            throw new ArgumentException("Invalid value for searchDirection");
        }
      }

      var hits = new List<DependencyObject>();

      VisualTreeHelper.HitTest(itemsControl, null,
                               result => {
                                 var itemContainer = isItemContainer
                                                       ? result.VisualHit.FindVisualParent(itemContainerType, itemsControl)
                                                       : result.VisualHit.FindVisualParent(itemContainerType);
                                 if (itemContainer != null && !hits.Contains(itemContainer) && ((UIElement)itemContainer).IsVisible == true) {
                                   hits.Add(itemContainer);
                                 }
                                 return HitTestResultBehavior.Continue;
                               },
                               new GeometryHitTestParameters(hitTestGeometry));

      return GetClosest(itemsControl, hits, position, searchDirection);
    }

    public static Type GetItemContainerType(this ItemsControl itemsControl, out bool isItemContainer)
    {
      // determines if the itemsControl is not a ListView, ListBox or TreeView
      isItemContainer = false;

      if (typeof(DataGrid).IsAssignableFrom(itemsControl.GetType())) {
        return typeof(DataGridRow);
      }

      // There is no safe way to get the item container type for an ItemsControl. 
      // First hard-code the types for the common ItemsControls.
      //if (itemsControl.GetType().IsAssignableFrom(typeof(ListView)))
      if (typeof(ListView).IsAssignableFrom(itemsControl.GetType())) {
        return typeof(ListViewItem);
      }
        //if (itemsControl.GetType().IsAssignableFrom(typeof(ListBox)))
      else if (typeof(ListBox).IsAssignableFrom(itemsControl.GetType())) {
        return typeof(ListBoxItem);
      }
        //else if (itemsControl.GetType().IsAssignableFrom(typeof(TreeView)))
      else if (typeof(TreeView).IsAssignableFrom(itemsControl.GetType())) {
        return typeof(TreeViewItem);
      }

      // Otherwise look for the control's ItemsPresenter, get it's child panel and the first 
      // child of that *should* be an item container.
      //
      // If the control currently has no items, we're out of luck.
      if (itemsControl.Items.Count > 0) {
        var itemsPresenters = itemsControl.GetVisualDescendents<ItemsPresenter>();

        foreach (var itemsPresenter in itemsPresenters) {
          var panel = VisualTreeHelper.GetChild(itemsPresenter, 0);
          var itemContainer = VisualTreeHelper.GetChildrenCount(panel) > 0
                                ? VisualTreeHelper.GetChild(panel, 0)
                                : null;

          // Ensure that this actually *is* an item container by checking it with
          // ItemContainerGenerator.
          if (itemContainer != null &&
              itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) != -1) {
            isItemContainer = true;
            return itemContainer.GetType();
          }
        }
      }

      return null;
    }

    public static Orientation GetItemsPanelOrientation(this ItemsControl itemsControl)
    {
      var itemsPresenter = itemsControl.FindVisualDescendant<ItemsPresenter>();

      if (itemsPresenter != null) {
        var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
        var orientationProperty = itemsPanel.GetType().GetProperty("Orientation", typeof(Orientation));

        if (orientationProperty != null) {
          return (Orientation)orientationProperty.GetValue(itemsPanel, null);
        }
      }

      // Make a guess!
      return Orientation.Vertical;
    }

    public static FlowDirection GetItemsPanelFlowDirection(this ItemsControl itemsControl)
    {
      var itemsPresenter = itemsControl.FindVisualDescendant<ItemsPresenter>();

      if (itemsPresenter != null) {
        var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
        var flowDirectionProperty = itemsPanel.GetType().GetProperty("FlowDirection", typeof(FlowDirection));

        if (flowDirectionProperty != null) {
          return (FlowDirection)flowDirectionProperty.GetValue(itemsPanel, null);
        }
      }

      // Make a guess!
      return FlowDirection.LeftToRight;
    }

    public static void SetSelectedItem(this ItemsControl itemsControl, object item)
    {
      if (itemsControl is MultiSelector) {
        ((MultiSelector)itemsControl).SelectedItem = null;
        ((MultiSelector)itemsControl).SelectedItem = item;
      } else if (itemsControl is ListBox) {
        ((ListBox)itemsControl).SelectedItem = null;
        ((ListBox)itemsControl).SelectedItem = item;
      } else if (itemsControl is TreeView) {
        // TODO: Select the TreeViewItem
        //((TreeView)itemsControl)
      } else if (itemsControl is Selector) {
        ((Selector)itemsControl).SelectedItem = null;
        ((Selector)itemsControl).SelectedItem = item;
      }
    }

    public static void SetItemSelected(this ItemsControl itemsControl, object item, bool value)
    {
      if (itemsControl is MultiSelector) {
        var multiSelector = (MultiSelector)itemsControl;

        if (value) {
          if (multiSelector.CanSelectMultipleItems()) {
            multiSelector.SelectedItems.Add(item);
          } else {
            multiSelector.SelectedItem = item;
          }
        } else {
          multiSelector.SelectedItems.Remove(item);
        }
      } else if (itemsControl is ListBox) {
        var listBox = (ListBox)itemsControl;

        if (value) {
          if (listBox.SelectionMode != SelectionMode.Single) {
            listBox.SelectedItems.Add(item);
          } else {
            listBox.SelectedItem = item;
          }
        } else {
          listBox.SelectedItems.Remove(item);
        }
      }
    }

    static UIElement GetClosest(
        ItemsControl itemsControl, 
        List<DependencyObject> items,
        Point position, 
        Orientation searchDirection)
    {
        UIElement closest = null;

        var closestDistance = double.MaxValue;

        foreach (var i in items)
        {
            var uiElement = i as UIElement;

            if (uiElement != null)
            {
                if (!itemsControl.IsAncestorOf(uiElement))
                    continue;

                var p = uiElement.TransformToAncestor(itemsControl).Transform(new Point(0, 0));

                var distance = double.MaxValue;

                if (itemsControl is TreeView)
                {
                    var xDiff = position.X - p.X;
                    var yDiff = position.Y - p.Y;
                    var hyp = Math.Sqrt(Math.Pow(xDiff, 2d) + Math.Pow(yDiff, 2d));
                    distance = Math.Abs(hyp);
                }
                else
                {
                    switch (searchDirection)
                    {
                        case Orientation.Horizontal:
                            distance = Math.Abs(position.X - p.X);
                            break;
                        case Orientation.Vertical:
                            distance = Math.Abs(position.Y - p.Y);
                            break;
                    }
                }

                if (distance < closestDistance)
                {
                    closest = uiElement;
                    closestDistance = distance;
                }
            }
        }

        return closest;
    }
  }
}