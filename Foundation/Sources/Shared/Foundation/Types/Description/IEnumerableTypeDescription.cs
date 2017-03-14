using SquaredInfinity.Collections;
using SquaredInfinity.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Types.Description
{
    public interface IEnumerableTypeDescription : ITypeDescription
    {
        Type DefaultConcreteItemType { get; }

        bool CanSetCapacity { get; }

        void SetCapacity(object obj, int capacity);

        bool CanAcceptItemType(Type itemType);
    }
}
