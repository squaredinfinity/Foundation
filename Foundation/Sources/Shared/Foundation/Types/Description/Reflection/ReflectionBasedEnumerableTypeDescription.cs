using SquaredInfinity.Collections;
using SquaredInfinity.Collections.Concurrent;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using System.Reflection;

namespace SquaredInfinity.Types.Description.Reflection
{
    public class ReflectionBasedEnumerableTypeDescription : ReflectionBasedTypeDescription, IEnumerableTypeDescription
    {
        public Type DefaultConcreteItemType { get; set; }

        public bool CanSetCapacity { get; set; }

        public PropertyInfo CapacityPropertyInfo { get; set; }

        readonly ConcurrentDictionary<Type, bool> ItemTypeCompatibility = new ConcurrentDictionary<Type, bool>();

        public bool CanAcceptItemType(Type itemType)
        {
            var isCompatible = false;

            if (ItemTypeCompatibility.TryGetValue(itemType, out isCompatible))
            {
                return isCompatible;
            }

            // no cached data about this item type compatibility
            // check if it is compatible or not

            isCompatible = Type.CanAcceptItem(itemType);

            ItemTypeCompatibility.AddOrUpdate(itemType, isCompatible);

            return isCompatible;
        }

        public void AddCompatibleItemType(Type itemType)
        {
            ItemTypeCompatibility.GetOrAdd(itemType, true);
        }

        public void AddIncompatibleItemType(Type itemType)
        {
            ItemTypeCompatibility.GetOrAdd(itemType, false);
        }

        public void SetCapacity(object obj, int capacity)
        {
            CapacityPropertyInfo.SetValue(obj, capacity);
        }
    }
}
