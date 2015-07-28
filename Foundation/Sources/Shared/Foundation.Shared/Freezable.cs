using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public class Freezable : IFreezable
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

        protected virtual bool TrySetThisPropertyValue<T>(
            ref T backingField,
            T value)
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
