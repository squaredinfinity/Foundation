using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Collections
{
    public class ObjectPoolItemAcquisition<TItem> : IObjectPoolItemAcquisition<TItem>
    {
        TItem _item;
        public TItem Item
        {
            get { return _item; }
            private set { _item = value; }
        }

        bool _isItemTransient;
        public bool IsItemTransient
        {
            get { return _isItemTransient; }
            private set { _isItemTransient = value; }
        }

        IObjectPoolStore<TItem> _itemOwner;
        public IObjectPoolStore<TItem> ItemOwner
        {
            get { return _itemOwner; }
            private set { _itemOwner = value; }
        }

        public ObjectPoolItemAcquisition(IObjectPoolStore<TItem> itemOwner, TItem item, bool isItemTransient)
        {
            this.ItemOwner = itemOwner;
            this.Item = item;
            this.IsItemTransient = isItemTransient;
        }

        public void Dispose()
        {
            ItemOwner.Release(this);
        }
    }
}
