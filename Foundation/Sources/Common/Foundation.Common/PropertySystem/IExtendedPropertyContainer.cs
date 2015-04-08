using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public interface IExtendedPropertyContainer
    {
        IExtendedPropertyContainer Parent { get; set; }

        IExtendedPropertyCollection ExtendedProperties { get; }

        /// <summary>
        /// Gets value of a property from inheritance tree.
        /// This container and its children will not be checked for the property, only property containers above in inheritance tree will.
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <param name="inheritedValue"></param>
        /// <returns></returns>
        bool TryGetInheritedPropertyValue(string uniqueName, out object inheritedValue);

        /// <summary>
        /// Gets the actual value of requested property.
        /// If property exists in this container and its value is set, then that value will be returned.
        /// If property does not exist in this container or exists but its value is not set, then inheritace tree will be walked up until a value is found.
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <param name="value"></param>
        /// <returns>True if property with set value could be found, false otherwise.</returns>
        bool TryGetActualPropertyValue(string uniqueName, out object actualValue);
    }

}
