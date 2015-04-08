using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public abstract class ExtendedPropertyContainer : IExtendedPropertyContainer
    {
        public IExtendedPropertyContainer Parent { get; set; }

        IExtendedPropertyCollection _extendedProperties;
        public IExtendedPropertyCollection ExtendedProperties
        {
            get { return _extendedProperties; }
        }

        public virtual bool TryGetInheritedPropertyValue(string uniqueName, out object inheritedValue)
        {
            if (Parent == null)
            {
                inheritedValue = null;
                return false;
            }
            else
            {
                if (Parent.TryGetActualPropertyValue(uniqueName, out inheritedValue))
                {
                    return true;
                }

                return false;
            }
        }

        public virtual bool TryGetActualPropertyValue(string uniqueName, out object actualValue)
        {
            return ExtendedProperties.TryGetActualPropertyValue(uniqueName, out actualValue);
        }

        public ExtendedPropertyContainer()
        {
            _extendedProperties = new ExtendedPropertyCollection(this);
        }
    }

}
