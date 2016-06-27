﻿using SquaredInfinity.Foundation.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace SquaredInfinity.Foundation.Presentation
{
    public abstract class SelectorLogic : NotifyPropertyChangedObject, ISelectorLogic
    {
        public virtual IReadOnlyList<IUserAction> GetAvailableUserAction(object selectorIdentifier, object item)
        {
            return new IUserAction[0];
        }

        public virtual Color? GetItemBackgroundColor(object selectorIdentifier, object item)
        {
            var group = GetItemGroup(selectorIdentifier, item);

            if (group != null)
                return group.Color;

            return null;
        }

        public virtual IAdaptiveSelectorItemGroup GetItemGroup(object selectorIdentifier, object item)
        {
            return null;
        }

        public virtual Visibility GetSelectionMarkersVisibility(int allItemsCount, int selectedItemsCount)
        {            
            // 0 or 1 item, don't show markers
            if (allItemsCount < 2)
            {
                return Visibility.Collapsed;
            }

            // not all items selected, show markers
            if (selectedItemsCount < allItemsCount)
            {
                return Visibility.Visible;
            }
            else // all items selected, don't show markers
            {
                return Visibility.Collapsed;
            }
        }

        public virtual void OnItemSelected(object selectorIdentifier, object selectedItem)
        { }

        public virtual void OnItemUnselected(object selectorIdentifier, object unselectedItem)
        { }
    }
}
