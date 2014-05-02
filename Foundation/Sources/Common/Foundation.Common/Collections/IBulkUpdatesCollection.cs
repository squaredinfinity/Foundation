using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public interface IBulkUpdatesCollection<TItem>
    {
        void AddRange(IEnumerable<TItem> items);

        void RemoveRange(int index, int count);

        void Reset(IEnumerable<TItem> newItems);
    }
}
