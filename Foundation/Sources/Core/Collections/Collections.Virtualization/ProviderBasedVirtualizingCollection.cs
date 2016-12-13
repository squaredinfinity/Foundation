using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Collections
{

    public class ProviderBasedVirtualizingCollection<TDataItem>
     : IList<IVirtualizedDataItem<TDataItem>>, IList
        where TDataItem : class
    {
        readonly PageCollection<TDataItem> _pages = new PageCollection<TDataItem>();
        protected PageCollection<TDataItem> Pages
        {
            get { return _pages; }
        }

        VirtualizedDataItemsPage<TDataItem> EnsurePage(int pageNumber)
        {
            var isNew = false;

            var page =
                Pages.GetOrAdd(pageNumber, (_) =>
                {
                    isNew = true;

                    var newPage = new VirtualizedDataItemsPage<TDataItem>(pageNumber * PageSize, PageSize);

                    return newPage;
                });

            if (isNew)
            {
                var pageItems = ItemsProvider.GetItems(pageNumber * PageSize, PageSize);

                page.ReloadItems(pageItems);
            }

            return page;
        }

        public IVirtualizedDataItemsProvider<TDataItem> ItemsProvider { get; private set; }
        public int PageSize { get; private set; }
        public TimeSpan PageLoadTimeout { get; private set; }
        
        public ProviderBasedVirtualizingCollection(IVirtualizedDataItemsProvider<TDataItem> itemsProvider)
            : this(itemsProvider, pageSize: 1, pageLoadTimeout: TimeSpan.FromSeconds(5))
        { }

        public ProviderBasedVirtualizingCollection(IVirtualizedDataItemsProvider<TDataItem> itemsProvider, int pageSize, TimeSpan pageLoadTimeout)
        {
            this.ItemsProvider = itemsProvider;
            this.PageSize = pageSize;
            this.PageLoadTimeout = pageLoadTimeout;

            ItemsProvider.AfterDataChanged += ItemsProvider_AfterDataChanged;
        }

        void ItemsProvider_AfterDataChanged(object sender, EventArgs e)
        {
            HandleUnderlyingDataChange();
        }

        protected virtual void HandleUnderlyingDataChange()
        { }

        protected virtual void CleanUp()
        {
            Pages.Clear();
        }

        #region IList<IVirtualizedDataItem<TDataItem>>

        public int IndexOf(IVirtualizedDataItem<TDataItem> item)
        {
            foreach (var kvp in Pages)
            {
                var index_onPage = kvp.Value.Items.IndexOf(item);

                if (index_onPage >= 0)
                {
                    return (kvp.Key * PageSize) + index_onPage;
                }
            }

            return -1;
        }

        public void Insert(int index, IVirtualizedDataItem<TDataItem> item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public IVirtualizedDataItem<TDataItem> this[int index]
        {
            get
            {
                // get the page number from item index
                var pageNumber = index / PageSize;

                // get the index on the page
                var index_onPage = index % PageSize;

                // load the page
                var page = EnsurePage(pageNumber);

                // TODO: load next / previous page (using Cache Length property, may need to be added)

                // update count (in case it changed)
                Count = ItemsProvider.GetTotalItemsCount();

                // TODO: remove pages which are no longer used                

                return page.Items[index_onPage];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void Add(IVirtualizedDataItem<TDataItem> item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(IVirtualizedDataItem<TDataItem> item)
        {
            foreach (var page in Pages.Values)
            {
                if (page.Items.Contains(item))
                    return true;
            }

            return false;
        }

        public void CopyTo(IVirtualizedDataItem<TDataItem>[] array, int arrayIndex)
        {
            var copy = this.ItemsProvider.GetItems(0, ItemsProvider.GetTotalItemsCount());
            Array.Copy(copy.ToArray(), array, copy.Count);
        }

        int _count = -1;

        public int Count
        {
            get
            {
                if (_count < 0)
                {
                    _count = ItemsProvider.GetTotalItemsCount();
                }

                return _count;
            }

            protected set
            {
                _count = value;
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(IVirtualizedDataItem<TDataItem> item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<IVirtualizedDataItem<TDataItem>> GetEnumerator()
        {
            yield break;
        }

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
            return Contains(value as IVirtualizedDataItem<TDataItem>);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf(value as IVirtualizedDataItem<TDataItem>);
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
                throw new NotSupportedException();
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            var copy = this.ItemsProvider.GetItems(0, ItemsProvider.GetTotalItemsCount());
            Array.Copy(copy.ToArray(), array, copy.Count);
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
}
