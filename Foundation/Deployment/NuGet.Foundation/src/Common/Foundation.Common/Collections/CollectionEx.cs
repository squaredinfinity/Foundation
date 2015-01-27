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
    public class CollectionEx<TItem> : 
        Collection<TItem>,
        ICollectionEx<TItem>,
        IBulkUpdatesCollection<TItem>, 
        INotifyCollectionContentChanged,
        System.Collections.IList
    {
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

        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
                return;

            MoveItem(oldIndex, newIndex);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            using (CollectionLock.AcquireWriteLock())
            {
                TItem item = this[oldIndex];

                base.RemoveItem(oldIndex);

                base.InsertItem(newIndex, item);
                RaiseVersionChanged();
            }
        }

        protected override void ClearItems()
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.ClearItems();

                RaiseVersionChanged();
            }
        }

        protected override void InsertItem(int index, TItem item)
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.InsertItem(index, item);

                RaiseVersionChanged();
            }
        }

        protected override void RemoveItem(int index)
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.RemoveItem(index);

                RaiseVersionChanged();
            }
        }

        protected override void SetItem(int index, TItem item)
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.SetItem(index, item);

                RaiseVersionChanged();
            }
        }

        public virtual void AddRange(IEnumerable<TItem> items)
        {
            if (items == null)
                return;

            using(CollectionLock.AcquireWriteLock())
            {
                foreach (var item in items)
                    Items.Add(item);

                RaiseVersionChanged();
            }
        }

        public virtual void RemoveRange(int index, int count)
        {
            using(CollectionLock.AcquireWriteLock())
            {
                for (int i = index; i < index + count; i++)
                    Items.RemoveAt(index);

                RaiseVersionChanged();
            }
        }

        public virtual void Replace(TItem oldItem, TItem newItem)
        {
            using(CollectionLock.AcquireUpgradeableReadLock())
            {
                var oldIndex = IndexOf(oldItem);

                if (oldIndex < 0)
                    return;

                SetItem(oldIndex, newItem);
            }
        }

        public virtual void Reset(IEnumerable<TItem> newItems)
        {
            using(CollectionLock.AcquireWriteLock())
            {
                Items.Clear();

                foreach (var item in newItems)
                    Items.Add(item);

                RaiseVersionChanged();
            }
        }

        void RaiseVersionChanged()
        {
            if (State == STATE__BULKUPDATE)
                return;

            var newVersion = Interlocked.Increment(ref _version);

            OnVersionChanged();
        }

        protected virtual void OnVersionChanged() 
        {
            if (VersionChanged != null)
                VersionChanged(this, new CollectionContentChangedEventArgs(Version));
        }

        public event EventHandler<CollectionContentChangedEventArgs> VersionChanged;

        int _version;
        public int Version
        {
            get { return _version; }
        }


        public IReadOnlyList<TItem> GetSnapshot()
        {
            using(CollectionLock.AcquireReadLock())
            {
                var snapshot = this.ToArray();

                return snapshot;
            }
        }

        const int STATE__NORMAL = 0;
        const int STATE__BULKUPDATE = 1;

        int State = STATE__NORMAL;

        public IBulkUpdate BeginBulkUpdate()
        {
            if(Interlocked.CompareExchange(ref State, STATE__BULKUPDATE, STATE__NORMAL) != STATE__NORMAL)
            {
                throw new Exception("Bulk Update Operation has already started");
            }

            return new BulkUpdate(this);
        }

        public void EndBulkUpdate(IBulkUpdate bulkUpdate)
        {
            bulkUpdate.Dispose();

            if (Interlocked.CompareExchange(ref State, STATE__NORMAL, STATE__BULKUPDATE) != STATE__BULKUPDATE)
            {
                throw new Exception("Bulk Update Operation has already ended");
            }

            RaiseVersionChanged();
        }

        public void AddRange(IEnumerable items)
        {
            AddRange(items.Cast<TItem>());
        }

        public void Reset(IEnumerable newItems)
        {
            Reset(newItems.Cast<TItem>());
        }
    }

    class BulkUpdate : IBulkUpdate
    {
        readonly IBulkUpdatesCollection Owner;
        bool HasFinished = false;

        public BulkUpdate(IBulkUpdatesCollection owner)
        {
            this.Owner = owner;
        }

        public void Dispose()
        {
            if (HasFinished)
                return;

            HasFinished = true;
            Owner.EndBulkUpdate(this);
        }
    }
}
