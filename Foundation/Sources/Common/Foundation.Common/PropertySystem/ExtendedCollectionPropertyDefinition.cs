using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public class ExtendedCollectionPropertyDefinition<TItem> : IExtendedCollectionPropertyDefinition<TItem>
    {
        public ExtendedPropertyUniqueIdentifier Id { get; private set; }
        public Func<IList<TItem>> GetDefaultValue { get; private set; }

        public ExtendedCollectionPropertyDefinition(string ownerUniqueName, string propertyName, Func<IList<TItem>> getDefaultValue)
        {
            this.Id = new ExtendedPropertyUniqueIdentifier(ownerUniqueName, propertyName);
            this.GetDefaultValue = getDefaultValue;
        }

        public IList<TItem> GetActualValue(IExtendedPropertyContainer container)
        {
            var result = (object)null;

            EnsurePropertyRegisteredWithContainer(container);

            container.TryGetActualPropertyValue(this, out result);

            return (IList<TItem>)result;

        }

        public void SetValue(IExtendedPropertyContainer container, object value)
        {
            container.ExtendedProperties.GetOrAddCollectionProperty(this).Value = value;
        }

        public void UnsetValue(IExtendedPropertyContainer container)
        {
            container.ExtendedProperties.GetOrAddCollectionProperty(this).IsValueSet = false;
        }

        public void SetValue(IExtendedPropertyContainer container)
        {
            container.ExtendedProperties.GetOrAddCollectionProperty(this).IsValueSet = true;
        }


        public bool GetIsValueSet(IExtendedPropertyContainer container)
        {
            EnsurePropertyRegisteredWithContainer(container);

            return container.ExtendedProperties.GetOrAddCollectionProperty(this).IsValueSet;
        }

        public IExtendedProperty EnsurePropertyRegisteredWithContainer(IExtendedPropertyContainer container)
        {
            var property = container.ExtendedProperties.GetOrAddCollectionProperty(this);

            return property;
        }

        public void SetInheritanceMode(IExtendedPropertyContainer container, CollectionInheritanceMode inheritanceMode)
        {
            var property = container.ExtendedProperties.GetOrAddCollectionProperty(this);
            property.InheritanceMode = inheritanceMode;
        }
    }
}
