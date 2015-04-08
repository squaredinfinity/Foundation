using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    public interface IExtendedPropertyCollection
    {
        /// <summary>
        /// Gets value of a property from inheritance tree.
        /// This collection will not be checked for the property, only property containers above in inheritance tree will.
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <param name="inheritedValue"></param>
        /// <returns></returns>
        bool TryGetInheritedPropertyValue(string uniqueName, out object inheritedValue);

        /// <summary>
        /// Gets the actual value of requested property.
        /// If property exists in this collection and its value is set, then that value will be returned.
        /// If property does not exist in this collection or exists but its value is not set, then inheritace tree will be walked up until a value is found.
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <param name="value"></param>
        /// <returns>True if property with set value could be found, false otherwise.</returns>
        bool TryGetActualPropertyValue(string uniqueName, out object value);

        IExtendedProperty<T> RegisterProperty<T>(string uniqueName);
        IExtendedProperty<T> RegisterProperty<T>(string uniqueName, Func<T> getDefaultValue);


        CollectionExtendedProperty<TItem> RegisterCollectionProperty<TItem>(string uniqueName, Func<Collection<TItem>> getDefaultValue);

        IExtendedProperty this[string uniqueName] { get; }
    }

}
