using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization
{
    public interface IEnumerableTypeSerializationStrategy<T, I> : ITypeSerializationStrategy<T>
        where T : IEnumerable<I>
    {
        IEnumerableTypeSerializationStrategy<T, I> ElementFilter(Predicate<I> filterElement);
    }
}
