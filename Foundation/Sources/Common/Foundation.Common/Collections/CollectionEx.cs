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
    public class CollectionEx<TItem> : 
        Collection<TItem>,
        ICollectionEx<TItem>,
        IBulkUpdatesCollection<TItem>, 
        INotifyCollectionVersionChanged,
        System.Collections.IList
    {
        /// <summary>
        /// Lock providing atomic access fo elements in collection
        /// </summary>
        readonly protected ILock CollectionLock;

        object IList.this[int index] { get { return this[index]; } set { this[index] = (TItem) value; } }

        public CollectionEx()
        {
            CollectionLock = new ReaderWriterLockSlimEx(LockRecursionPolicy.NoRecursion);
        }

        public CollectionEx(IList<TItem> items)
            : base(items)
        {
            CollectionLock = new ReaderWriterLockSlimEx(LockRecursionPolicy.NoRecursion);
        }

        public CollectionEx(LockRecursionPolicy recursionPolicy)
        {
            CollectionLock = new ReaderWriterLockSlimEx(recursionPolicy);
        }

        public CollectionEx(LockRecursionPolicy recursionPolicy, IList<TItem> items)
            : base(items)
        {
            CollectionLock = new ReaderWriterLockSlimEx(recursionPolicy);
        }

        #region Move

        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
                return;

            MoveItem(oldIndex, newIndex);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            using (CollectionLock.AcquireWriteLockIfNotHeld())
            {
                TItem item = this[oldIndex];

                // remove item from old intext
                base.RemoveItem(oldIndex);

                // insert item in a new index
                base.InsertItem(newIndex, item);

                OnAfterItemMoved(item, oldIndex, newIndex);

                IncrementVersion();
            }
        }

        protected virtual void OnAfterItemMoved(TItem item, int oldIndex, int newIndex)
        { }

        #endregion

        #region Clear

        protected override void ClearItems()
        {
            using (CollectionLock.AcquireWriteLockIfNotHeld())
            {
                var oldItems = new TItem[Count];
                CopyTo(oldItems, 0);

                OnBeforeItemsCleared(oldItems);
                base.ClearItems();
                OnAfterItemsCleared(oldItems);

                IncrementVersion();
            }
        }

        protected virtual void OnBeforeItemsCleared(IReadOnlyList<TItem> oldItems)
        { }

        protected virtual void OnAfterItemsCleared(IReadOnlyList<TItem> oldItems)
        { }

        #endregion

        #region Insert

        protected override void InsertItem(int index, TItem item)
        {
            using(CollectionLock.AcquireWriteLockIfNotHeld())
            {
                OnBeforeItemInserted(index, item);
                base.InsertItem(index, item);
                OnAfterItemInserted(index, item);

                IncrementVersion();
            }
        }

        protected virtual void OnBeforeItemInserted(int index, TItem item) { }

        protected virtual void OnAfterItemInserted(int index, TItem item) { }

        #endregion

        #region Remove

        protected override void RemoveItem(int index)
        {
            using (var readLock = CollectionLock.AcquireWriteLockIfNotHeld())
            {
                var item = this[index];

                OnBeforeItemRemoved(item, index);
                base.RemoveItem(index);
                OnAfterItemRemoved(item, index);

                IncrementVersion();
            }
        }

        protected virtual void OnBeforeItemRemoved(TItem item, int index) { }

        protected virtual void OnAfterItemRemoved(TItem item, int index) { }

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

                base.SetItem(index, newItem);

                OnAfterItemReplaced(index, oldItem, newItem);

                IncrementVersion();
            }
        }

        protected override void SetItem(int index, TItem newItem)
        {
            using (CollectionLock.AcquireWriteLockIfNotHeld())
            {
                if (index < 0)
                    throw new IndexOutOfRangeException("specified item does not exist in the collection.");

                var oldItem = this[index];

                OnBeforeItemReplaced(index, oldItem, newItem);

                base.SetItem(index, newItem);

                OnAfterItemReplaced(index, oldItem, newItem);
                IncrementVersion();
            }
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
                    InsertItem(Items.Count, item);
                }
            }
        }

        public virtual void RemoveRange(int index, int count)
        {
            using (var blkUpdate = BeginBulkUpdate())
            {
                for (int i = index; i < index + count; i++)
                {
                    RemoveAt(index);
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

            if(LockAcquisition != null)
                LockAcquisition.Dispose();

            HasFinished = true;
            Owner.EndBulkUpdate(this);
        }
    }
}
