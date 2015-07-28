using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{

    public class ExtendedProperty<T> : NotifyPropertyChangedObject, IExtendedProperty<T>
    {
        IExtendedPropertyDefinition _propertyDefinition;
        public IExtendedPropertyDefinition PropertyDefinition
        {
            get { return _propertyDefinition; }
            private set { _propertyDefinition = value; }
        }

        public IExtendedPropertyCollection Owner { get; private set; }

        public event EventHandler<EventArgs> AfterActualValueChanged;
        void RaiseAfterActualValueChanged()
        {
            if (AfterActualValueChanged != null)
                AfterActualValueChanged(this, EventArgs.Empty);

            RaisePropertyChanged(() => ActualValue);
        }

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
                    RaiseAfterActualValueChanged();
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
                if (CanValueBeInherited)
                {
                    object inheritedValue = null;
                    if (Owner.TryGetInheritedPropertyValue(PropertyDefinition, out inheritedValue))
                    {
                        return (T)inheritedValue;
                    }
                    else
                    {
                        AssignDefaultValue();
                        return Value;
                    }
                }
                else
                {
                    AssignDefaultValue();
                    return Value;
                }
            }
        }

        protected void AssignDefaultValue()
        {
            if (object.Equals(_value, default(T)))
                _value = GetDefaultValue();
        }

        protected Func<T> GetDefaultValue { get; private set; }

        protected bool CanValueBeInherited { get; private set; }

        internal ExtendedProperty(IExtendedPropertyCollection owner, IExtendedPropertyDefinition propertyDefinition, Func<T> getDefaultValue, bool canValueBeInherited)
        {
            this.Owner = owner;
            this.PropertyDefinition = propertyDefinition;
            this.Value = getDefaultValue();
            // using default value, do not mark Value as set.
            this.IsValueSet = false;
            this.GetDefaultValue = getDefaultValue;
            this.CanValueBeInherited = canValueBeInherited;
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
