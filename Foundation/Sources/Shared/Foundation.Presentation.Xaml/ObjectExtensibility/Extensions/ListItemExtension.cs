using SquaredInfinity.ObjectExtensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.ObjectExtensibility.Extensions
{
    public class ListItemExtension : NotifyPropertyChangedObjectExtension<PresentationWrapper>
    {
        public event EventHandler AfterIsSelectedChanged;

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            {
                if(TrySetThisPropertyValue(ref _isSelected, value))
                {
                    if (AfterIsSelectedChanged != null)
                        AfterIsSelectedChanged(this, EventArgs.Empty);
                }
            }
        }
    }
}
