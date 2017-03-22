using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.ComponentModel
{
    public class FreezableNotifyPropertyChangedObject : NotifyPropertyChangedObject, IFreezable
    {
        bool _isFrozen = false;
        public bool IsFrozen
        {
            get { return _isFrozen; }
            private set { _isFrozen = value; }
        }

        public void Freeze()
        {
            IsFrozen = true;
        }

        protected override bool TrySetThisPropertyValue<T>(
            ref T backingField, 
            T value, 
            bool raisePropertyChanged = true, 
            [CallerMemberName] string propertyName = null)
        {
            if (IsFrozen)
                return false;

            if (object.Equals(backingField, value))
                return false;

            backingField = value;

            return true;
        }
    }
}
