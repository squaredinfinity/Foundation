using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization
{
    public interface IEnumerableTypeSerializationStrategy<T, TItem> : ITypeSerializationStrategy<T>
    {
        IEnumerableTypeSerializationStrategy<T, TItem> ElementFilter(Predicate<TItem> elementFilter);
    }
}
