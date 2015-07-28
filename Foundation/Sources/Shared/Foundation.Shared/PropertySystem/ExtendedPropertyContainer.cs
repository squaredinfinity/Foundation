using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public abstract class ExtendedPropertyContainer : NotifyPropertyChangedObject, IExtendedPropertyContainer
    {
        public IExtendedPropertyContainer Parent { get; set; }

        IExtendedPropertyCollection _extendedProperties;
        public IExtendedPropertyCollection ExtendedProperties
        {
            get { return _extendedProperties; }
        }

        public virtual bool TryGetInheritedPropertyValue(IExtendedPropertyDefinition propertyDefinition, out object inheritedValue)
        {
            if (Parent == null)
            {
                inheritedValue = null;
                return false;
            }
            else
            {
                if (Parent.TryGetActualPropertyValue(propertyDefinition, out inheritedValue))
                {
                    return true;
                }

                return false;
            }
        }

        public virtual bool TryGetActualPropertyValue(IExtendedPropertyDefinition propertyDefinition, out object actualValue)
        {
            propertyDefinition.EnsurePropertyRegisteredWithContainer(this);

            return ExtendedProperties.TryGetActualPropertyValue(propertyDefinition, out actualValue);
        }

        public ExtendedPropertyContainer()
        {
            _extendedProperties = new ExtendedPropertyCollection(this);
        }
    }

}
