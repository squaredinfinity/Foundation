using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation.Controls.AdaptiveSelector
{
    public class AdaptiveSelectorItemGroup : NotifyPropertyChangedObject, IAdaptiveSelectorItemGroup
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { TrySetThisPropertyValue(ref _name, value); }
        }

        Color? _color;
        public Color? Color
        {
            get { return _color; }
            set { TrySetThisPropertyValue(ref _color, value); }
        }
    }
}
