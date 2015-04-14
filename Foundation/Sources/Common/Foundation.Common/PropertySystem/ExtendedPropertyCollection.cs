using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public class ExtendedPropertyCollection : IExtendedPropertyCollection
    {
        readonly Dictionary<ExtendedPropertyUniqueIdentifier, IExtendedProperty> Properties
            = new Dictionary<ExtendedPropertyUniqueIdentifier, IExtendedProperty>();

        public IExtendedPropertyContainer Owner { get; private set; }
        
        public bool TryGetActualPropertyValue(IExtendedPropertyDefinition propertyDefinition, out object value)
        {
            var prop = (IExtendedProperty)null;

            if (Properties.TryGetValue(propertyDefinition.Id, out prop))
            {
                value = prop.ActualValue;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public bool TryGetInheritedPropertyValue(IExtendedPropertyDefinition propertyDefinition, out object inheritedValue)
        {
            if (Owner.TryGetInheritedPropertyValue(propertyDefinition, out inheritedValue))
                return true;
            else
                return false;
        }

        public ExtendedPropertyCollection(IExtendedPropertyContainer owner)
        {
            this.Owner = owner;
        }

        public IExtendedProperty this[ExtendedPropertyUniqueIdentifier propertyId]
        {
            get
            {
                var prop = (IExtendedProperty)null;

                if (Properties.TryGetValue(propertyId, out prop))
                {
                    return prop;
                }
                else
                {
                    throw new ArgumentException("Property with id '{0}' does not exist.".FormatWith(propertyId));
                }
            }
        }

        public IExtendedProperty this[string propertyFullName]
        {
            get
            {
                var propertyId = new ExtendedPropertyUniqueIdentifier(propertyFullName);

                var prop = (IExtendedProperty)null;

                if (Properties.TryGetValue(propertyId, out prop))
                {
                    return prop;
                }
                else
                {
                    throw new ArgumentException("Property with id '{0}' does not exist.".FormatWith(propertyId));
                }
            }
        }


        public IExtendedProperty GetOrAddProperty<T>(IExtendedPropertyDefinition<T> propertyDefinition)
        {
            var property = (IExtendedProperty)null;

            if (Properties.TryGetValue(propertyDefinition.Id, out property))
            {
                return property;
            }
            else
            {
                property = new ExtendedProperty<T>(this, propertyDefinition, propertyDefinition.GetDefaultValue, propertyDefinition.CanValueBeInherited);
                Properties.Add(propertyDefinition.Id, property);
                return property;
            }
        }


        public ICollectionExtendedProperty GetOrAddCollectionProperty<T>(ExtendedCollectionPropertyDefinition<T> propertyDefinition)
        {
            var property = (IExtendedProperty)null;

            if (Properties.TryGetValue(propertyDefinition.Id, out property))
            {
                return (ICollectionExtendedProperty)property;
            }
            else
            {
                property = new CollectionExtendedProperty<T>(this, propertyDefinition, propertyDefinition.GetDefaultValue, propertyDefinition.CanValueBeInherited);
                Properties.Add(propertyDefinition.Id, property);
                return (ICollectionExtendedProperty)property;
            }
        }
    }
}
