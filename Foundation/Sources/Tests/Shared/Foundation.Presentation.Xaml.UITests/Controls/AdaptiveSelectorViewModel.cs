using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Presentation;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using SquaredInfinity.Foundation.Presentation.Controls;
using SquaredInfinity.Foundation.Presentation.Controls.AdaptiveSelector;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.Controls
{
    public class AdaptiveSelectorViewModel : ViewModel
    {
        readonly List<int> _numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public List<int> Numbers { get { return _numbers; } }

        readonly XamlObservableCollectionEx<int> _selectedNumbers = new XamlObservableCollectionEx<int>();
        public XamlObservableCollectionEx<int> SelectedNumbers { get { return _selectedNumbers; } }

        CustomSelectorLogic _selectorLogic = new CustomSelectorLogic();
        public CustomSelectorLogic SelectorLogic { get { return _selectorLogic; } }
    }

    public class CustomSelectorLogic : SelectorLogic
    {
        object ModeOneItem;
        object ModeTwoItem;

        Color ModeOneColor = Colors.DarkRed;
        Color ModeTwoColor = Colors.DarkSlateBlue;

        // this behavior allows up to two selected items to have custom mode (Mode 1 and Mode 2).
        // different modes will be distinguished by colour (Red and Blue)
        // First selected item wil lalways be mode 1, second will always be mode 2 (if mode from first item have not been removed)

        public override void OnItemSelected(AdaptiveSelector selector, object selectorIdentifier, object selectedItem)
        {
            base.OnItemSelected(selector, selectorIdentifier, selectedItem);

            if (object.Equals(ModeOneItem, selectedItem) || object.Equals(ModeTwoItem, selectedItem))
                return;

            if (ModeOneItem == null)
            {
                ModeOneItem = selectedItem;
            }
            else if(ModeTwoItem == null)
            {
                ModeTwoItem = selectedItem;
            }
        }

        public override void OnItemUnselected(AdaptiveSelector selector, object selectorIdentifier, object unselectedItem)
        {
            if(object.Equals(ModeOneItem, unselectedItem))
            {
                ModeOneItem = null;
            }
            else if(object.Equals(ModeTwoItem, unselectedItem))
            {
                ModeTwoItem = null;
            }

            base.OnItemUnselected(selector, selectorIdentifier, unselectedItem);
        }

        public override Color? GetItemBackgroundColor(AdaptiveSelector selector, object selectorIdentifier, object item, bool isSelected)
        {
            if (object.Equals(ModeOneItem, item))
                return ModeOneColor;

            if (object.Equals(ModeTwoItem, item))
                return ModeTwoColor;

            return base.GetItemBackgroundColor(selector, selectorIdentifier, item, isSelected);
        }

        public override IReadOnlyList<IUserAction> GetAvailableUserAction(AdaptiveSelector selector, object selectorIdentifier, object item)
        {
            var result = new List<IUserAction>();

            var ua = new UserAction("set Mode One", (x) =>
            {
                selector.UnselectItem(ModeOneItem);

                ModeOneItem = item;

                selector.EnsureItemSelected(item);
            });

            if(!object.Equals(item, ModeOneItem))
                result.Add(ua);

            ua = new UserAction("set Mode Two", (x) =>
            {
                selector.UnselectItem(ModeTwoItem);

                ModeTwoItem = item;

                selector.EnsureItemSelected(item);
            });

            if (!object.Equals(item, ModeTwoItem))
                result.Add(ua);

            ua = new UserAction("swap", (x) =>
            {
                var one = ModeOneItem;
                var two = ModeTwoItem;

                selector.UnselectItem(one);
                selector.UnselectItem(two);

                ModeOneItem = two;
                ModeTwoItem = one;

                selector.EnsureItemSelected(ModeOneItem);
                selector.EnsureItemSelected(ModeTwoItem);
            });

            if(ModeOneItem != null && ModeTwoItem != null)
                result.Add(ua);

            return result;
        }
    }
}
