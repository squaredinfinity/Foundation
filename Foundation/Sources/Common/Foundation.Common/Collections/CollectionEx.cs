using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public class CollectionEx<TItem> : 
        Collection<TItem>, 
        IBulkUpdatesCollection<TItem>, 
        INotifyCollectionContentChanged
    {
        readonly ILock UpdateLock = new ReaderWriterLockSlimEx();

        public CollectionEx()
        { }

        public CollectionEx(IList<TItem> items)
            : base(items)
        { }

        protected override void ClearItems()
        {
            using (UpdateLock.AcquireWriteLock())
            {
                base.ClearItems();

                OnVersionChangedInternal();
            }
        }

        protected override void InsertItem(int index, TItem item)
        {
            using (UpdateLock.AcquireWriteLock())
            {
                base.InsertItem(index, item);

                OnVersionChangedInternal();
            }
        }

        protected override void RemoveItem(int index)
        {
            using (UpdateLock.AcquireWriteLock())
            {
                base.RemoveItem(index);

                OnVersionChangedInternal();
            }
        }

        protected override void SetItem(int index, TItem item)
        {
            using (UpdateLock.AcquireWriteLock())
            {
                base.SetItem(index, item);

                OnVersionChangedInternal();
            }
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            using(UpdateLock.AcquireWriteLock())
            {
                foreach (var item in items)
                    Items.Add(item);

                OnVersionChangedInternal();
            }
        }

        public void RemoveRange(int index, int count)
        {
            using(UpdateLock.AcquireWriteLock())
            {
                for (int i = index; i < index + count; i++)
                    Items.RemoveAt(index);

                OnVersionChangedInternal();
            }
        }

        public void Reset(IEnumerable<TItem> newItems)
        {
            using(UpdateLock.AcquireWriteLock())
            {
                Items.Clear();

                OnVersionChangedInternal();
            }
        }

        void OnVersionChangedInternal()
        {
            OnVersionChanged();
        }

        protected virtual void OnVersionChanged() 
        {
        }

        public event Action<INotifyCollectionContentChanged> VersionChanged;

        int _version;
        public int Version
        {
            get { return _version; }
        }
    }
}
