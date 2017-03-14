using SquaredInfinity.Foundation.Threading;
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
        ILock Lock { get; }
        new int Count { get; }

        void Replace(TItem oldItem, TItem newItem);

        /// <summary>
        /// Removes old items and replaces them with new items in one, atomic operation
        /// </summary>
        /// <param name="newItems"></param>
        void Reset(IEnumerable<TItem> newItems);
        IReadOnlyList<TItem> GetSnapshot();

        void CopyTo(int index, TItem[] array, int arrayIndex, int count);
    }

    public interface ICollectionEx : IBulkUpdatesCollection
    {
        void Move(int oldIndex, int newIndex);
        void RemoveAt(int index);
    }
}
