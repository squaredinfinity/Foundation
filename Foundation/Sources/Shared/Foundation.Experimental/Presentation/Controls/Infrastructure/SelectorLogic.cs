using SquaredInfinity.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Reactive.Subjects;
using SquaredInfinity.Presentation.Controls.AdaptiveSelector;

namespace SquaredInfinity.Presentation
{
    public abstract class SelectorLogic : NotifyPropertyChangedObject, ISelectorLogic
    {
        public virtual IReadOnlyList<IUserAction> GetAvailableUserAction(AdaptiveSelector selector, object selectorIdentifier, object item)
        {
            return new IUserAction[0];
        }

        readonly Subject<EventArgs> _itemBackgroundRefreshRequested = new Subject<EventArgs>();
        public IObservable<EventArgs> ItemBackgroundRefreshRequested
        {
            get { return _itemBackgroundRefreshRequested; }
        }

        public void RaiseItemBackgroundRefreshRequest()
        {
            _itemBackgroundRefreshRequested.OnNext(EventArgs.Empty);
        }

        public virtual Color? GetItemBackgroundColor(AdaptiveSelector selector, object selectorIdentifier, object item, bool isSelected)
        {
            var group = GetItemGroup(selector, selectorIdentifier, item);

            if (group != null)
                return group.Color;

            return null;
        }

        public virtual IAdaptiveSelectorItemGroup GetItemGroup(AdaptiveSelector selector, object selectorIdentifier, object item)
        {
            return null;
        }

        public virtual Visibility GetSelectionMarkersVisibility(AdaptiveSelector selector, object selectorIdentifier, int allItemsCount, int selectedItemsCount)
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

        public virtual void OnItemSelected(AdaptiveSelector selector, object selectorIdentifier, object selectedItem)
        { }

        public virtual void OnItemUnselected(AdaptiveSelector selector, object selectorIdentifier, object unselectedItem)
        { }
    }
}
