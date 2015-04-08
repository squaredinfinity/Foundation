using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public class ExtendedProperty<T> : NotifyPropertyChangedObject, IExtendedProperty<T>
    {
        string _uniqueName;
        public string UniqueName
        {
            get { return _uniqueName; }
            private set { _uniqueName = value; }
        }

        public IExtendedPropertyCollection Owner { get; private set; }

        bool _isValueSet = false;
        /// <summary>
        /// True if a value of this property is set.
        /// False if value is not set, in which case inherited value will be used (or a default value if inherited value is not set)
        /// </summary>
        public bool IsValueSet
        {
            get { return _isValueSet; }
            set
            {
                if (TrySetThisPropertyValue(ref _isValueSet, value))
                {
                    RaisePropertyChanged(() => ActualValue);
                }
            }
        }

        T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                TrySetThisPropertyValue(ref _value, value);

                IsValueSet = true;
            }
        }

        public T ActualValue
        {
            get
            {
                return GetActualValue();
            }
        }

        protected virtual T GetActualValue()
        {
            if (IsValueSet)
            {
                return Value;
            }
            else
            {
                object inheritedValue = null;
                if (Owner.TryGetInheritedPropertyValue(UniqueName, out inheritedValue))
                {
                    return (T)inheritedValue;
                }
                else
                {
                    return GetDefaultValue();
                }
            }
        }

        protected Func<T> GetDefaultValue { get; private set; }

        public ExtendedProperty(IExtendedPropertyCollection owner, string uniqueName, Func<T> getDefaultValue)
        {
            this.Owner = owner;
            this.UniqueName = uniqueName;
            this.Value = getDefaultValue();
            // using default value, do not mark Value as set.
            this.IsValueSet = false;
            this.GetDefaultValue = getDefaultValue;
        }


        object IExtendedProperty.Value
        {
            get { return this.Value; }
            set { this.Value = (T)value; }
        }

        object IExtendedProperty.ActualValue
        {
            get { return this.ActualValue; }
        }
    }
}
