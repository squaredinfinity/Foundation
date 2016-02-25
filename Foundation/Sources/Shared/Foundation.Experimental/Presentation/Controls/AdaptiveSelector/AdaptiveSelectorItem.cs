using SquaredInfinity.Foundation.Presentation.Controls.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Controls.AdaptiveSelector
{
    public class AdaptiveSelectorItem : NotifyPropertyChangedObject, IAdaptiveSelectorItem
    {
        string _displayName;
        public string DisplayName
        {
            get { return GetDisplayName(); }
            set { TrySetThisPropertyValue(ref _displayName, value); }
        }

        protected virtual string GetDisplayName()
        {
            return _displayName;
        }

        /// <summary>
        /// Returns a color of this item.
        /// Default behavior returns group color (if any)
        /// </summary>
        /// <returns></returns>
        public virtual Color? GetBackgroundColor()
        {
            if (Group != null)
            {
                return Group.Color;
            }

            return null;
        }

        public virtual IReadOnlyList<IUserAction> GetAvailableUserActions()
        {
            return new IUserAction[0];
        }

        IAdaptiveSelectorItemGroup _group;
        public IAdaptiveSelectorItemGroup Group
        {
            get { return _group; }
            set { TrySetThisPropertyValue(ref _group, value); }
        }

        object _item;
        public object Item
        {
            get { return _item; }
            set { TrySetThisPropertyValue(ref _item, value); }
        }
    }
}
