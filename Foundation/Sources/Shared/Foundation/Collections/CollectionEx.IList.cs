using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Foundation.Collections
{
    public partial class CollectionEx<TItem> :
        ICollectionEx<TItem>,
        IBulkUpdatesCollection<TItem>,
        INotifyCollectionVersionChanged,
        IList<TItem>,
        IList,
        ICollection<TItem>,
        ICollection
    {
        IList<TItem> _items;

        object _syncRoot;

        /// <summary>
        /// Gets the number of elements actually contained in the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The number of elements actually contained in the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </returns>
        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Collections.Generic.IList`1"/> wrapper around the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IList`1"/> wrapper around the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </returns>
        protected IList<TItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// 
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>. </exception>
        public TItem this[int index]
        {
            get { return _items[index]; }
            set
            {
                if (_items.IsReadOnly)
                    throw new NotSupportedException();

                using (CollectionLock.AcquireWriteLockIfNotHeld())
                {
                    if (index < 0 || index >= _items.Count)
                        throw new ArgumentOutOfRangeException();

                    Replace(index, value);
                }
            }
        }

        bool ICollection<TItem>.IsReadOnly
        {
            get { return _items.IsReadOnly; }
        }

        bool ICollection.IsSynchronized
        {
            // todo: in future this may be configurable via constructor
            get { return true; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    var col = _items as ICollection;
                    if (col != null)
                        _syncRoot = col.SyncRoot;
                    else
                        Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
                }

                return _syncRoot;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return (object)_items[index];
            }
            set
            {
                this[index] = (TItem)value;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return this._items.IsReadOnly;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return _items.IsReadOnly;
            }
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="T:System.Collections.ObjectModel.Collection`1"/>. The value can be null for reference types.</param>
        public void Add(TItem item)
        {
            using (CollectionLock.AcquireWriteLockIfNotHeld())
            {
                var index = _items.Count;

                Insert(index, item);
            }
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        public void Clear()
        {
            using (CollectionLock.AcquireWriteLockIfNotHeld())
            {
                var oldItems = new TItem[Count];
                CopyTo(oldItems, 0);

                OnBeforeItemsCleared(oldItems);

                for (int i = oldItems.Length - 1; i >= 0; i--)
                    OnBeforeItemRemoved(i, oldItems[i]);

                ClearItems();

                OnAfterItemsCleared(oldItems);

                for (int i = oldItems.Length - 1; i >= 0; i--)
                    OnAfterItemRemoved(i, oldItems[i]);

                IncrementVersion();
            }
        }

        /// <summary>
        /// Copies the entire <see cref="T:System.Collections.ObjectModel.Collection`1"/> to a compatible one-dimensional <see cref="T:System.Array"/>, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ObjectModel.Collection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.ObjectModel.Collection`1"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(TItem[] array, int index)
        {
            _items.CopyTo(array, index);
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        /// 
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.ObjectModel.Collection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.ObjectModel.Collection`1"/>. The value can be null for reference types.</param>
        public bool Contains(TItem item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        /// 
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerator`1"/> for the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </returns>
        public IEnumerator<TItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="T:System.Collections.ObjectModel.Collection`1"/>, if found; otherwise, -1.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.List`1"/>. The value can be null for reference types.</param>
        public int IndexOf(TItem item)
        {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert. The value can be null for reference types.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        public void Insert(int index, TItem item)
        {
            using (CollectionLock.AcquireWriteLockIfNotHeld())
            {
                if (index < 0 || index > _items.Count)
                    throw new ArgumentOutOfRangeException();

                OnBeforeItemInserted(index, item);

                InsertItem(index, item);

                OnAfterItemInserted(index, item);

                IncrementVersion();
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        /// 
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false.  This method also returns false if <paramref name="item"/> was not found in the original <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>. The value can be null for reference types.</param>
        public bool Remove(TItem item)
        {
            using (var readLock = CollectionLock.AcquireWriteLockIfNotHeld())
            {
                int index = _items.IndexOf(item);

                if (index < 0)
                    return false;

                OnBeforeItemRemoved(index, item);

                RemoveItem(index);

                OnAfterItemRemoved(index, item);

                IncrementVersion();

                return true;
            }
        }


        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.-or-<paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        public void RemoveAt(int index)
        {
            using (var readLock = CollectionLock.AcquireWriteLockIfNotHeld())
            {
                var item = _items[index];

                OnBeforeItemRemoved(index, item);

                RemoveItem(index);

                OnAfterItemRemoved(index, item);

                IncrementVersion();
            }
        }
        
        int IList.Add(object value)
        {
            this.Add((TItem)value);

            return this.Count - 1;
        }

        bool IList.Contains(object value)
        {
            if (IsCompatibleWithCollection(value))
                return this.Contains((TItem)value);
            else
                return false;
        }
        
        int IList.IndexOf(object value)
        {
            if (IsCompatibleWithCollection(value))
                return this.IndexOf((TItem)value);
            else
                return -1;
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, (TItem)value);
        }

        void IList.Remove(object value)
        {
            
            if (!IsCompatibleWithCollection(value))
                return;

            this.Remove((TItem)value);
        }

        static bool IsCompatibleWithCollection(object value)
        {
            if (value is TItem)
                return true;

            if (value == null)
                return (object)default(TItem) == null;
            else
                return false;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (array.Rank != 1)
                throw new ArgumentException("array Rank must be 1");
            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException("array lower bound must be 0");
            if (index < 0)
                throw new IndexOutOfRangeException();
            if (array.Length - index < this.Count)
                throw new ArgumentException("array is too small");

            TItem[] array1 = array as TItem[];

            if (array1 != null)
            {
                _items.CopyTo(array1, index);
            }
            else
            {
                Type elementType = array.GetType().GetElementType();

                Type c = typeof(TItem);

                if (!elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType))
                    throw new ArgumentException("invalid array type");

                object[] objArray = array as object[];
                if (objArray == null)
                    throw new ArgumentException("invalid array type");

                int count = _items.Count;

                try
                {
                    for (int index1 = 0; index1 < count; ++index1)
                        objArray[index++] = (object)_items[index1];
                }
                catch (ArrayTypeMismatchException ex)
                {
                    throw new ArgumentException("invalid array type");
                }
            }
        }
    }
}
