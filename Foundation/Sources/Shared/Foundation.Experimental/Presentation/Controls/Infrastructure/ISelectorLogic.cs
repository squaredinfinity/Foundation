using SquaredInfinity.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections;
using System.Windows.Controls;
using SquaredInfinity.Presentation.Controls.AdaptiveSelector;

namespace SquaredInfinity.Presentation
{
    public interface ISelectorLogic : INotifyVersionChangedObject
    {
        void OnItemSelected(AdaptiveSelector selector, object selectorIdentifier, object selectedItem);
        void OnItemUnselected(AdaptiveSelector selector, object selectorIdentifier, object unselectedItem);
        IReadOnlyList<IUserAction> GetAvailableUserAction(AdaptiveSelector selector, object selectorIdentifier, object item);
        IAdaptiveSelectorItemGroup GetItemGroup(AdaptiveSelector selector, object selectorIdentifier, object item);
        Visibility GetSelectionMarkersVisibility(AdaptiveSelector selector, object selectorIdentifier, int allItemsCount, int selectedItemsCount);
        IObservable<EventArgs> ItemBackgroundRefreshRequested { get; }
        Color? GetItemBackgroundColor(AdaptiveSelector selector, object selectorIdentifier, object item, bool isSelected);
    }
}
