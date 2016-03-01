using SquaredInfinity.Foundation.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation
{
    public abstract class SelectorBehaviorController : NotifyPropertyChangedObject, ISelectorBehaviorController
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

        public virtual void OnItemSelected(object selectorIdentifier, object selectedItem)
        { }

        public virtual void OnItemUnselected(object selectorIdentifier, object unselectedItem)
        { }
    }
}
