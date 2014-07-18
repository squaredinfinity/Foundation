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
        readonly protected ILock CollectionLock = new ReaderWriterLockSlimEx(LockRecursionPolicy.SupportsRecursion);

        object IList.this[int index] { get { return this[index]; } set { this[index] = (TItem) value; } }

        public CollectionEx()
        { }

        public CollectionEx(IList<TItem> items)
            : base(items)
        { }

        protected override void ClearItems()
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.ClearItems();

                OnVersionChangedInternal();
            }
        }

        protected override void InsertItem(int index, TItem item)
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.InsertItem(index, item);

                OnVersionChangedInternal();
            }
        }

        protected override void RemoveItem(int index)
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.RemoveItem(index);

                OnVersionChangedInternal();
            }
        }

        protected override void SetItem(int index, TItem item)
        {
            using (CollectionLock.AcquireWriteLock())
            {
                base.SetItem(index, item);

                OnVersionChangedInternal();
            }
        }

        public virtual void AddRange(IEnumerable<TItem> items)
        {
            using(CollectionLock.AcquireWriteLock())
            {
                foreach (var item in items)
                    Items.Add(item);

                OnVersionChangedInternal();
            }
        }

        public virtual void RemoveRange(int index, int count)
        {
            using(CollectionLock.AcquireWriteLock())
            {
                for (int i = index; i < index + count; i++)
                    Items.RemoveAt(index);

                OnVersionChangedInternal();
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

                OnVersionChangedInternal();
            }
        }

        void OnVersionChangedInternal()
        {
            OnVersionChanged();
        }

        protected virtual void OnVersionChanged() 
        {
            var newVersion = Interlocked.Increment(ref _version);

            if (VersionChanged != null)
                VersionChanged(this, new CollectionContentChangedEventArgs(newVersion));
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
    }
}
