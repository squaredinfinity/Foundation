using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Presentation.Controls.AdaptiveSelector
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

        double _left = 0.0;
        public double Left
        {
            get { return _left; }
            set { TrySetThisPropertyValue(ref _left, value); }
        }

        double _width = 0.0;
        public double Width
        {
            get { return _width; }
            set { TrySetThisPropertyValue(ref _width, value); }
        }
    }
}
