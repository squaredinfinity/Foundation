using SquaredInfinity.Threading;
using SquaredInfinity.Threading.Locks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Collections
{
    public interface ICollectionEx<TItem> : 
        ISupportsAsyncBulkUpdate,
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

        #region GetSnapshot

        IReadOnlyList<TItem> GetSnapshot();
        IReadOnlyList<TItem> GetSnapshot(SyncOptions options);

        Task<IReadOnlyList<TItem>> GetSnapshotAsync();
        Task<IReadOnlyList<TItem>> GetSnapshotAsync(AsyncOptions options);       

        #endregion

        void CopyTo(int index, TItem[] array, int arrayIndex, int count);
    }

    public interface ICollectionEx : IBulkUpdatesCollection
    {
        void Move(int oldIndex, int newIndex);
        void RemoveAt(int index);
    }
}
