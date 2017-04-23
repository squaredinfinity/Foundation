using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Concurrent;
using SquaredInfinity.Extensions;
using SquaredInfinity.Types.Description.Reflection;

namespace SquaredInfinity.Types.Description.IL
{
    public partial class ILBasedEnumerableTypeDescription : IEnumerableTypeDescription
    {
        Type IEnumerableTypeDescription.DefaultConcreteItemType
        {
            get { return EnumerableSource.DefaultConcreteItemType; }
        }

        bool IEnumerableTypeDescription.CanSetCapacity
        {
            get { return EnumerableSource.CanSetCapacity; }
        }

        void IEnumerableTypeDescription.SetCapacity(object obj, int capacity)
        {
            EnumerableSource.SetCapacity(obj, capacity);
        }

        bool IEnumerableTypeDescription.CanAcceptItemType(Type itemType)
        {
            return EnumerableSource.CanAcceptItemType(itemType);
        }
    }
}
