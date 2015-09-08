using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public interface ICollectionEx<TItem> : 
        ICollection<TItem>,
        ICollectionEx,
        IReadOnlyList<TItem>
    {
        new int Count { get; }

        void Replace(TItem oldItem, TItem newItem);

        /// <summary>
        /// Removes old items and replaces them with new items in one, atomic operation
        /// </summary>
        /// <param name="newItems"></param>
        void Reset(IEnumerable<TItem> newItems);
        IReadOnlyList<TItem> GetSnapshot();
    }

    public interface ICollectionEx : IBulkUpdatesCollection
    {
        void Move(int oldIndex, int newIndex);
    }
}
