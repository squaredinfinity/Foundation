using SquaredInfinity.Foundation.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections;
using System.Windows.Controls;

namespace SquaredInfinity.Foundation.Presentation
{
    public interface ISelectorLogic : INotifyVersionChangedObject
    {
        void OnItemSelected(object selectorIdentifier, object selectedItem);
        void OnItemUnselected(object selectorIdentifier, object unselectedItem);
        IReadOnlyList<IUserAction> GetAvailableUserAction(object selectorIdentifier, object item);
        Color? GetItemBackgroundColor(object selectorIdentifier, object item);
        IAdaptiveSelectorItemGroup GetItemGroup(object selectorIdentifier, object item);
        Visibility GetSelectionMarkersVisibility(int allItemsCount, int selectedItemsCount);
    }
}
