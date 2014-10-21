using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    public abstract class VirtualizingCollection<TDataItem> :
        IList<TDataItem>,
        IList
        where TDataItem : class
    {
        public VirtualizingCollection()
        { }

        protected abstract int GetCount();
        protected abstract void SetCount(int newCount);

        protected abstract TDataItem GetItem(int index);

        #region IList<TDataItem>

        public abstract bool Contains(TDataItem item);

        public abstract int IndexOf(TDataItem item);

        public void Insert(int index, TDataItem item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public TDataItem this[int index]
        {
            get { return GetItem(index); }
            set { throw new NotSupportedException(); }
        }

        public void Add(TDataItem item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public void CopyTo(TDataItem[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get { return GetCount(); }
            set { SetCount(value); }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(TDataItem item)
        {
            throw new NotSupportedException();
        }

        public abstract IEnumerator<TDataItem> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IList

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value)
        {
            return Contains(value as TDataItem);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf(value as TDataItem);
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (TDataItem)value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        int ICollection.Count
        {
            get { return Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        #endregion
    }


    public interface IVirtualizedDataItem<TDataItem>
       where TDataItem : class
    {
        int Index { get; set; }
        bool IsLoading { get; set; }
        TDataItem DataItem { get; set; }
    }

    public class VirtualizedDataItem<TDataItem> : NotifyPropertyChangedObject, IVirtualizedDataItem<TDataItem>
        where TDataItem : class
    {
        int _index;
        /// <summary>
        /// Index of underlying Data Item in collection being virtualized.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { TrySetThisPropertyValue(ref _isLoading, value); }
        }

        TDataItem _dataItem;
        /// <summary>
        /// Underlying Data Item
        /// </summary>
        public TDataItem DataItem
        {
            get { return _dataItem; }
            set
            {
                if (TrySetThisPropertyValue(ref _dataItem, value))
                {
                    IsLoading = _dataItem == null;
                }
            }
        }

        public VirtualizedDataItem(int index)
        {
            this.Index = index;
        }
    }

    public interface IVirtualizedDataItemsProvider<TDataItem>
    {
        event EventHandler AfterDataChanged;

        /// <summary>
        /// Returns a total number of all items in an underlying collection being virtualized
        /// </summary>
        /// <returns></returns>
        int GetTotalItemsCount();

        IReadOnlyList<TDataItem> GetItems(int startIndex, int numberOfItems);
    }

    public class VirtualizedDataItemsPage<TDataItem>
        where TDataItem : class
    {
        IList<IVirtualizedDataItem<TDataItem>> _items;
        public IList<IVirtualizedDataItem<TDataItem>> Items
        {
            get { return _items; }
            private set { _items = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startIndex">start index of first item on this page in relation to a whole underlying collection being virtualized</param>
        /// <param name="pageSize">number of items in this page</param>
        public VirtualizedDataItemsPage(int startIndex, int pageSize)
        {
            Items = new List<IVirtualizedDataItem<TDataItem>>(capacity: pageSize);

            for (int i = 0; i < pageSize; i++)
            {
                Items.Add(new VirtualizedDataItem<TDataItem>(index: i));
            }
        }

        /// <summary>
        /// Updates this page with new items.
        /// </summary>
        /// <param name="newItems"></param>
        public void ReloadItems(IReadOnlyList<TDataItem> newItems)
        {
            //# Replace any existing items

            var to_count = Math.Min(newItems.Count, Items.Count);

            for (int i = 0; i < to_count; i++)
            {
                if (!object.Equals(Items[i], newItems[i]))
                    Items[i].DataItem = newItems[i];
            }

            //# Add new items

            var dataItem_index = Items[to_count - 1].Index;

            for (int i = to_count; i < newItems.Count; i++)
            {
                var di = new VirtualizedDataItem<TDataItem>(dataItem_index + i);
                di.DataItem = newItems[i];

                Items.Add(di);
            }

            //# Remove remaining old items
            for (int i = Items.Count - 1; i >= newItems.Count; i--)
            {
                Items.RemoveAt(i);
            }
        }
    }

    public class PageCollection<TDataItem> : ConcurrentDictionary<int, VirtualizedDataItemsPage<TDataItem>>
        where TDataItem : class
    { }
}
