using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    /// <summary>
    /// Thread-Safe collection with support for atomic reads/writes and additional operations such as GetSnapshot(), Bulk Updates (Reset, Add/Remove Range) and Versioning
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public partial class CollectionEx<TItem> : 
        ICollectionEx<TItem>,
        IBulkUpdatesCollection<TItem>, 
        INotifyCollectionVersionChanged,
        IList<TItem>,
        IList,
        ICollection<TItem>,
        ICollection
    {
        /// <summary>
        /// Lock providing atomic access fo elements in collection
        /// </summary>
        readonly protected ILock CollectionLock;
        
        public CollectionEx()
        {
            _items = new List<TItem>();
            CollectionLock = new ReaderWriterLockSlimEx(LockRecursionPolicy.NoRecursion);
        }

        public CollectionEx(IEnumerable<TItem> items)
        {
            _items = new List<TItem>(items);
            CollectionLock = new ReaderWriterLockSlimEx(LockRecursionPolicy.NoRecursion);
        }

        public CollectionEx(List<TItem> items)
        {
            _items = items;
            CollectionLock = new ReaderWriterLockSlimEx(LockRecursionPolicy.NoRecursion);
        }

        public CollectionEx(LockRecursionPolicy recursionPolicy)
        {
            _items = new List<TItem>();
            CollectionLock = new ReaderWriterLockSlimEx(recursionPolicy);
        }

        public CollectionEx(LockRecursionPolicy recursionPolicy, IEnumerable<TItem> items)
        {
            _items = new List<TItem>(items);
            CollectionLock = new ReaderWriterLockSlimEx(recursionPolicy);
        }

        public CollectionEx(LockRecursionPolicy recursionPolicy, List<TItem> items)
        {
            _items = items;
            CollectionLock = new ReaderWriterLockSlimEx(recursionPolicy);
        }

        int IReadOnlyCollection<TItem>.Count
        {
            get { return this.Count; }
        }

        int ICollection<TItem>.Count
        {
            get { return this.Count; }
        }



        #region Move

        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
                return;

            using (CollectionLock.AcquireWriteLockIfNotHeld())
            {
                TItem item = this[oldIndex];

                // todo: onbeforeitemmoved ?

                MoveItem(item, oldIndex, newIndex);

                OnAfterItemMoved(item, oldIndex, newIndex);

                IncrementVersion();
            }
        }

        protected virtual void MoveItem(TItem item, int oldIndex, int newIndex)
        {
            // remove item from old intext
            RemoveItem(oldIndex);

            // insert item in a new index
            InsertItem(newIndex, item);
        }

        protected virtual void OnAfterItemMoved(TItem item, int oldIndex, int newIndex)
        { }

        #endregion

        #region Clear

        protected virtual void ClearItems()
        {
            _items.Clear();
        }

        protected virtual void OnBeforeItemsCleared(IReadOnlyList<TItem> oldItems)
        { }

        protected virtual void OnAfterItemsCleared(IReadOnlyList<TItem> oldItems)
        { }

        #endregion

        #region Insert

        protected virtual void InsertItem(int index, TItem item)
        {
            _items.Insert(index, item);
        }

        protected virtual void OnBeforeItemInserted(int index, TItem item) { }

        protected virtual void OnAfterItemInserted(int index, TItem item) { }

        #endregion

        #region Remove

        protected virtual void RemoveItem(int index)
        {
            _items.RemoveAt(index);
        }

        protected virtual void OnBeforeItemRemoved(int index, TItem item) { }

        protected virtual void OnAfterItemRemoved(int index, TItem item, bool triggeredByClearOperation) { }

        #endregion

        #region Replace / Set Item

        public virtual void Replace(TItem oldItem, TItem newItem)
        {
            using (CollectionLock.AcquireWriteLockIfNotHeld())
            {
                var index = IndexOf(oldItem);

                if (index < 0)
                    throw new IndexOutOfRangeException("specified item does not exist in the collection.");

                OnBeforeItemReplaced(index, oldItem, newItem);
                OnBeforeItemRemoved(index, oldItem);
                OnBeforeItemInserted(index, newItem);

                SetItem(index, newItem);

                OnAfterItemReplaced(index, oldItem, newItem);
                OnAfterItemRemoved(index, oldItem, triggeredByClearOperation:false);
                OnAfterItemInserted(index, newItem);

                IncrementVersion();
            }
        }

        protected virtual void Replace(int index, TItem newItem)
        {
            using (CollectionLock.AcquireWriteLockIfNotHeld())
            {
                if (index < 0)
                    throw new IndexOutOfRangeException("specified item does not exist in the collection.");

                var oldItem = _items[index];

                OnBeforeItemReplaced(index, oldItem, newItem);
                OnBeforeItemRemoved(index, oldItem);
                OnBeforeItemInserted(index, newItem);

                SetItem(index, newItem);

                OnAfterItemReplaced(index, oldItem, newItem);
                OnAfterItemRemoved(index, oldItem, triggeredByClearOperation:false);
                OnAfterItemInserted(index, newItem);

                IncrementVersion();
            }
        }

        protected virtual void SetItem(int index, TItem newItem)
        {
            _items[index] = newItem;
        }

        protected virtual void OnAfterItemReplaced(int index, TItem oldItem, TItem newItem) { }

        protected virtual void OnBeforeItemReplaced(int index, TItem oldItem, TItem newItem) { }

        #endregion

        #region Bulk Updates (Add Range, Remove Range, Reset)

        public void AddRange(IEnumerable items)
        {
            AddRange(items.Cast<TItem>());
        }

        public void Reset(IEnumerable newItems)
        {
            Reset(newItems.Cast<TItem>());
        }

        public virtual void AddRange(IEnumerable<TItem> items)
        {
            if (items == null)
                return;

            using (var blkUpdate = BeginBulkUpdate())
            {
                foreach (var item in items)
                {
                    Insert(Items.Count, item);
                }
            }
        }

        public virtual void RemoveRange(int index, int count)
        {
            using (var blkUpdate = BeginBulkUpdate())
            {
                for (int i = index + count - 1; i >= index; i--)
                {
                    RemoveAt(i);
                }
            }
        }

        public virtual void Reset(IEnumerable<TItem> newItems)
        {
            using (var blkUpdate = BeginBulkUpdate())
            {
                Clear();

                AddRange(newItems);
            }

            OnAfterCollectionReset();
        }

        protected virtual void OnAfterCollectionReset() { }

        #endregion

        #region CopyTo

        public void CopyTo(int index, TItem[] array, int arrayIndex, int count)
        {
            _items.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>
        /// Copies the entire <see cref="T:System.Collections.ObjectModel.Collection`1"/> to a compatible one-dimensional <see cref="T:System.Array"/>, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ObjectModel.Collection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.ObjectModel.Collection`1"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(TItem[] array, int index)
        {
            _items.CopyTo(array, index);
        }

        #endregion

        public bool IsBulkUpdateInProgress()
        {
            return State == STATE__BULKUPDATE;
        }

        protected virtual void IncrementVersion()
        {
            if (State == STATE__BULKUPDATE)
                return;

            var newVersion = Interlocked.Increment(ref _version);

            OnVersionChanged(newVersion);
        }

        void OnVersionChanged(long newVersion) 
        {
            if (!CollectionLock.IsReadLockHeld && !CollectionLock.IsUpgradeableReadLockHeld && !CollectionLock.IsWriteLockHeld)
            {
                using (CollectionLock.AcquireReadLock())
                {
                    if (VersionChanged != null)
                        VersionChanged(this, new VersionChangedEventArgs(newVersion));

                    OnAfterVersionChanged(newVersion);
                }
            }
            else
            {
                if (VersionChanged != null)
                    VersionChanged(this, new VersionChangedEventArgs(newVersion));

                OnAfterVersionChanged(newVersion);
            }
        }

        /// <summary>
        /// Called after version of this collection has changed
        /// </summary>
        /// <param name="newVersion"></param>
        protected virtual void OnAfterVersionChanged(long newVersion) { }

        public event EventHandler<VersionChangedEventArgs> VersionChanged;

        long _version;
        public long Version
        {
            get { return _version; }
        }


        public IReadOnlyList<TItem> GetSnapshot()
        {
            using(CollectionLock.AcquireReadLockIfNotHeld())
            {
                var snapshot = this.ToArray();

                return snapshot;
            }
        }

        protected const int STATE__NORMAL = 0;
        protected const int STATE__BULKUPDATE = 1;

        protected int State = STATE__NORMAL;

        public bool IsInBulkUpdate
        {
            get { return State == STATE__BULKUPDATE; }
        }

        public IBulkUpdate BeginBulkUpdate()
        {
            var write_lock = CollectionLock.AcquireWriteLockIfNotHeld();

            // write lock != null => somebody else is doing update
            // make sure that we are in normal state, otherwsie there's a bug somewhere
            if(write_lock != null && Interlocked.CompareExchange(ref State, STATE__BULKUPDATE, STATE__NORMAL) != STATE__NORMAL)
            {
                throw new Exception("Bulk Update Operation has already started");
            }

            if (write_lock == null)
            {
                // bulk update already in progress
                return null;
            }
            else
            {
                // start new bulk update
                return new BulkUpdate(this, write_lock);
            }
        }

        public void EndBulkUpdate(IBulkUpdate bulkUpdate)
        {
            if (Interlocked.CompareExchange(ref State, STATE__NORMAL, STATE__BULKUPDATE) != STATE__BULKUPDATE)
            {
                throw new Exception("Bulk Update Operation has already ended");
            }

            IncrementVersion();

            bulkUpdate.Dispose();

            OnAfterBulkUpdate();
        }

        protected virtual void OnAfterBulkUpdate()
        {

        }
    }

    class BulkUpdate : IBulkUpdate
    {
        readonly IBulkUpdatesCollection Owner;
        readonly IWriteLockAcquisition LockAcquisition;
        bool HasFinished = false;

        public BulkUpdate(IBulkUpdatesCollection owner, IWriteLockAcquisition lockAcquisition)
        {
            this.Owner = owner;
            LockAcquisition = lockAcquisition;
        }

        public void Dispose()
        {
            if (HasFinished)
                return;

            HasFinished = true;
            Owner.EndBulkUpdate(this);

            if (LockAcquisition != null)
                LockAcquisition.Dispose();
        }
    }
}