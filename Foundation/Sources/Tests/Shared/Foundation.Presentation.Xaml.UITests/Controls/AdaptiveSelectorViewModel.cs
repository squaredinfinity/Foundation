using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Presentation;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.Controls
{
    public class AdaptiveSelectorViewModel : ViewModel
    {
        readonly List<int> _numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public List<int> Numbers { get { return _numbers; } }

        readonly XamlObservableCollectionEx<int> _selectedNumbers = new XamlObservableCollectionEx<int>();
        public XamlObservableCollectionEx<int> SelectedNumbers { get { return _selectedNumbers; } }

        CustomSelectorBehaviorController _selectorBehaviorController = new CustomSelectorBehaviorController();
        public CustomSelectorBehaviorController SelectorBehaviorController { get { return new CustomSelectorBehaviorController(); } }
    }

    public class CustomSelectorBehaviorController : SelectorBehaviorController
    {
        object ModeOneItem;
        object ModeTwoItem;

        Color ModeOneColor = Colors.DarkRed;
        Color ModeTwoColor = Colors.DarkSlateBlue;

        // this behavior allows up to two selected items to have custom mode (Mode 1 and Mode 2).
        // different modes will be distinguished by colour (Red and Blue)
        // First selected item wil lalways be mode 1, second will always be mode 2 (if mode from first item have not been removed)

        public override void OnItemSelected(object selectorIdentifier, object selectedItem)
        {
            base.OnItemSelected(selectorIdentifier, selectedItem);

            if (ModeOneItem == null)
            {
                ModeOneItem = selectedItem;
            }
            else if(ModeTwoItem == null)
            {
                ModeTwoItem = selectedItem;
            }
        }

        public override void OnItemUnselected(object selectorIdentifier, object unselectedItem)
        {
            if(object.Equals(ModeOneItem, unselectedItem))
            {
                ModeOneItem = null;
            }
            else if(object.Equals(ModeTwoItem, unselectedItem))
            {
                ModeTwoItem = null;
            }

            base.OnItemUnselected(selectorIdentifier, unselectedItem);
        }

        public override Color? GetItemBackgroundColor(object selectorIdentifier, object item)
        {
            if (object.Equals(ModeOneItem, item))
                return ModeOneColor;

            if (object.Equals(ModeTwoItem, item))
                return ModeTwoColor;

            return base.GetItemBackgroundColor(selectorIdentifier, item);
        }
    }
}
