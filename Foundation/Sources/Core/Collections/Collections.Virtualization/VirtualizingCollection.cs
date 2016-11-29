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
            for(int i = 0; i < GetCount(); i++)
            {
                var item = GetItem(i);
                array[i + arrayIndex] = item;
            }
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
}
