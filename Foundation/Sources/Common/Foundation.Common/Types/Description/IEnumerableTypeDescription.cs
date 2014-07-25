using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Types.Description
{
    public interface IEnumerableTypeDescription : ITypeDescription
    {
        IReadOnlyList<Type> CompatibleItemTypes { get; }

        Type DefaultConcreteItemType { get; }

        bool CanSetCapacity { get; }

        void SetCapacity(object obj, int capacity);
    }
}
