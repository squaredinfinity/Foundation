using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Serialization
{
    public interface IEnumerableTypeSerializationStrategy<TEnumerableSerializationStrategy, TSerializationStrategy, T, TItem> 
        : ITypeSerializationStrategy<TSerializationStrategy, T>
        where TEnumerableSerializationStrategy : class, IEnumerableTypeSerializationStrategy<TEnumerableSerializationStrategy, TSerializationStrategy, T, TItem>
        where TSerializationStrategy : class, ITypeSerializationStrategy<TSerializationStrategy, T>
    {
        TEnumerableSerializationStrategy ElementFilter(Predicate<TItem> elementFilter);
    }
}
