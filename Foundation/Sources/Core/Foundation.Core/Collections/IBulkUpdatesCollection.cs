using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public interface IBulkUpdatesCollection : ISupportsBulkUpdate
    {
        void AddRange(IEnumerable items);

        void RemoveRange(int index, int count);

        void Reset(IEnumerable newItems);
    }

    public interface IBulkUpdatesCollection<TItem> : IBulkUpdatesCollection
    {
        void AddRange(IEnumerable<TItem> items);

        void Reset(IEnumerable<TItem> newItems);
    }
}
