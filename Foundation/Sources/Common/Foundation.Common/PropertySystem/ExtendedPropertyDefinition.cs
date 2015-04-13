using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public class ExtendedPropertyDefinition<T> : IExtendedPropertyDefinition<T>
    {
        public ExtendedPropertyUniqueIdentifier Id { get; private set; }
        public Func<T> GetDefaultValue { get; private set; }

        public ExtendedPropertyDefinition(string ownerUniqueName, string propertyName, Func<T> getDefaultValue)
        {
            this.Id = new ExtendedPropertyUniqueIdentifier(ownerUniqueName, propertyName);
            this.GetDefaultValue = getDefaultValue;
        }

        public T GetActualValue(IExtendedPropertyContainer container)
        {
            var result = (object)null;

            container.ExtendedProperties.GetOrAddProperty(this);

            container.TryGetActualPropertyValue(this, out result);

            return (T)result;

        }

        public void SetValue(IExtendedPropertyContainer container, object value)
        {
            container.ExtendedProperties.GetOrAddProperty(this).Value = value;
        }

        public void UnsetValue(IExtendedPropertyContainer container)
        {
            container.ExtendedProperties.GetOrAddProperty(this).IsValueSet = false;
        }

        public void SetValue(IExtendedPropertyContainer container)
        {
            container.ExtendedProperties.GetOrAddProperty(this).IsValueSet = true;
        }


        public bool GetIsValueSet(IExtendedPropertyContainer container)
        {
            EnsurePropertyRegisteredWithContainer(container);

            return container.ExtendedProperties.GetOrAddProperty(this).IsValueSet;
        }

        public IExtendedProperty EnsurePropertyRegisteredWithContainer(IExtendedPropertyContainer container)
        {
            var property = container.ExtendedProperties.GetOrAddProperty(this);

            return property;
        }
    }
}
