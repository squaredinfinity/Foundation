using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Controls.AdaptiveSelector
{
    public class SelectionMarker : NotifyPropertyChangedObject
    {
        object _item;
        public object Item
        {
            get { return _item; }
            set { TrySetThisPropertyValue(ref _item, value); }
        }

        bool _isDragging = false;
        public bool IsDragging
        {
            get { return _isDragging; }
            set { TrySetThisPropertyValue(ref _isDragging, value); }
        }
    }
}
