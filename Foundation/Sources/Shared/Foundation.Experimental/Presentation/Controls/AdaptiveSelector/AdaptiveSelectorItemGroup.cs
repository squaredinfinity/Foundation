using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Controls.AdaptiveSelector
{
    public class AdaptiveSelectorItemGroup : NotifyPropertyChangedObject, IAdaptiveSelectorItemGroup
    {
        string _uniqueName;
        public string UniqueName
        {
            get { return _uniqueName; }
            set { TrySetThisPropertyValue(ref _uniqueName, value); }
        }

        string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set { TrySetThisPropertyValue(ref _displayName, value); }
        }

        Color? _color;
        public Color? Color
        {
            get { return _color; }
            set { TrySetThisPropertyValue(ref _color, value); }
        }

        public override int GetHashCode()
        {
            return UniqueName.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as IAdaptiveSelectorItemGroup);
        }

        public bool Equals(IAdaptiveSelectorItemGroup other)
        {
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            return string.Equals(UniqueName, other.UniqueName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
