using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections.Concurrent
{
    public class ConcurrentHashSet<TItem> : IConcurrentCollectionEx<TItem>
    {
        readonly ILock CollectionLock = new ReaderWriterLockSlimEx();

        readonly HashSet<TItem> InternalSet;

        public ConcurrentHashSet()
        {
            InternalSet = new HashSet<TItem>();
        }

        public bool TryAdd(TItem item)
        {
            using(CollectionLock.AcquireWriteLock())
            {
                if (InternalSet.Contains(item))
                    return false;

                InternalSet.Add(item);
                return true;
            }
        }

        public IReadOnlyList<TItem> GetSnapshot()
        {
            using(CollectionLock.AcquireReadLock())
            {
                var snapshot = InternalSet.ToArray();

                return snapshot;
            }
        }


        public bool Contains(TItem item)
        {
            using(CollectionLock.AcquireReadLock())
            {
                return InternalSet.Contains(item);
            }
        }


        public void Reset(IEnumerable<TItem> newItems)
        {
            using(CollectionLock.AcquireWriteLock())
            {
                InternalSet.Clear();

                foreach(var item in newItems)
                {
                    InternalSet.Add(item);
                }
            }
        }

        public int Count
        {
            get { return InternalSet.Count; }
        }
    }

    class SnapshotEnumerator<TItem> : IEnumerator<TItem>
    {
        readonly IReadOnlyList<TItem> Snapshot;

        readonly IEnumerator<TItem> InternalEnumerator;

        public SnapshotEnumerator(IReadOnlyList<TItem> snapshot)
        {
            this.Snapshot = snapshot;

            this.InternalEnumerator = Snapshot.GetEnumerator();
        }

        public TItem Current
        {
            get { return InternalEnumerator.Current; }
        }

        public void Dispose()
        {
            InternalEnumerator.Dispose();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return InternalEnumerator.Current; }
        }

        public bool MoveNext()
        {
           return InternalEnumerator.MoveNext();
        }

        public void Reset()
        {
            InternalEnumerator.Reset();
        }
    }
}
