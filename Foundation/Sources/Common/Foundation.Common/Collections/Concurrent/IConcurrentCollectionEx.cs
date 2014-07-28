using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections.Concurrent
{
    public interface IConcurrentCollectionEx<TItem>
    {
        int Count { get; }

        bool TryAdd(TItem item);

        IReadOnlyList<TItem> GetSnapshot();

        bool Contains(TItem item);

        void Reset(IEnumerable<TItem> newItems);
    }
}
