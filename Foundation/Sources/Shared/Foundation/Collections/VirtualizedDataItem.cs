using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Collections
{
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
}
