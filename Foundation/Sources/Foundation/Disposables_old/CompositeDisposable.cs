using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Disposables
{
    public class CompositeDisposable : ICollection<IDisposable>, IEnumerable<IDisposable>, IEnumerable, IDisposable
    {
        readonly object Sync = new object();

        List<IDisposable> Store;

        public int Count
        {
            get { return Store.Count; }
        }

        public bool IsReadOnly { get; } = false;
        
        public bool IsDisposed { get; private set; }

        public CompositeDisposable()
        {
            this.Store = new List<IDisposable>();
        }

        public CompositeDisposable(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            this.Store = new List<IDisposable>(capacity);
        }
        
        public CompositeDisposable(params IDisposable[] disposables)
        {
            if (disposables == null)
                throw new ArgumentNullException(nameof(disposables));

            this.Store = new List<IDisposable>(disposables);
        }

        public CompositeDisposable(IEnumerable<IDisposable> disposables)
        {
            if (disposables == null)
                throw new ArgumentNullException(nameof(disposables));

            this.Store = new List<IDisposable>(disposables);
        }

        public void Add(IDisposable item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            var lock_taken = false;

            try
            {
                Monitor.Enter(Sync, ref lock_taken);

                if (IsDisposed)
                    item.Dispose();
                else
                    Store.Add(item);
            }
            finally
            {
                if (lock_taken)
                    Monitor.Exit(Sync);
            }
            
        }

        public bool Remove(IDisposable item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            var lock_taken = false;
            var item_removed = false;

            try
            {
                Monitor.Enter(Sync, ref lock_taken);

                item_removed = Store.Remove(item);
            }
            finally
            {
                if (lock_taken)
                    Monitor.Exit(Sync);
            }

            if (item_removed)
                item.Dispose();

            return item_removed;
        }

        public void Dispose()
        {
            var lock_taken = false;
            
            try
            {
                Monitor.Enter(Sync, ref lock_taken);

                if (IsDisposed)
                {
                    // nothing else to do here
                }
                else
                { 
                    Clear();
                    IsDisposed = true;
                }
            }
            finally
            {
                if (lock_taken)
                    Monitor.Exit(Sync);
            }
        }

        public void Clear()
        {
            bool lock_taken = false;

            try
            {
                Monitor.Enter(Sync, ref lock_taken);

                if (IsDisposed)
                {
                    // nothing else to do here
                }
                else
                {
                    for (int i = 0; i < Store.Count; i++)
                    {
                        Store[i].Dispose();
                    }

                    Store.Clear();
                }
            }
            finally
            {
                if (lock_taken)
                    Monitor.Exit(Sync);
            }
        }

        public bool Contains(IDisposable item)
        {
            var lock_taken = false;

            try
            {
                Monitor.Enter(Sync, ref lock_taken);

                return Store.Contains(item);
            }
            finally
            {
                if (lock_taken)
                    Monitor.Exit(Sync);
            }
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            var lock_taken = false;

            try
            {
                Monitor.Enter(Sync, ref lock_taken);

                Store.CopyTo(array, arrayIndex);
            }
            finally
            {
                if (lock_taken)
                    Monitor.Exit(Sync);
            }
        }

        /// </returns>
        public IEnumerator<IDisposable> GetEnumerator()
        {
            var lock_taken = false;

            try
            {
                Monitor.Enter(Sync, ref lock_taken);

                return Store.ToList().GetEnumerator();
            }
            finally
            {
                if (lock_taken)
                    Monitor.Exit(Sync);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var lock_taken = false;

            try
            {
                Monitor.Enter(Sync, ref lock_taken);

                return Store.ToArray().GetEnumerator();
            }
            finally
            {
                if (lock_taken)
                    Monitor.Exit(Sync);
            }
        }
    }
}
