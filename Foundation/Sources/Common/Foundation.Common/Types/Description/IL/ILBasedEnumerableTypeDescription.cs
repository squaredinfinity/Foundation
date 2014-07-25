using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SquaredInfinity.Foundation.Types.Description.IL
{
    public class ILBasedEnumerableTypeDescription : ILBasedTypeDescription, IEnumerableTypeDescription
    {
        IReadOnlyList<Type> _compatibleItemTypes;
        public IReadOnlyList<Type> CompatibleItemTypes
        {
            get { return _compatibleItemTypes; }
            set { _compatibleItemTypes = value; }
        }

        Type _defaultConcreteItemType;
        public Type DefaultConcreteItemType
        {
            get { return _defaultConcreteItemType; }
            set { _defaultConcreteItemType = value; }
        }


        bool _canSetCapacity;
        public bool CanSetCapacity
        {
            get { return _canSetCapacity; }
            set { _canSetCapacity = value; }
        }

        public PropertyInfo CapacityPropertyInfo;

        public void SetCapacity(object obj, int capacity)
        {
            CapacityPropertyInfo.SetValue(obj, capacity);
        }
    }
}
