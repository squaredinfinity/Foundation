using SquaredInfinity.Threading.Locks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Collections
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class IndexFilter<TItem> : IIndexFilter<TItem>
    {
        //  TODO PERFORMANCE:
        //      difference between using(Lock?.Acquire..() and Lock.Acquire..() with NoLock provided)

        public static IIndexFilter<TItem> AllIncluded(IReadOnlyList<TItem> wrapped) => new IndexFilter<TItem>(wrapped, indexes: null);
        public static IIndexFilter<TItem> AllExcluded(IReadOnlyList<TItem> wrapped) => new IndexFilter<TItem>(wrapped, new int[0]);

        readonly IReadOnlyList<TItem> Wrapped;

        //  IMPLEMENTATION:
        //      sequence of indexes may be important, so use list instead of say hashset
        readonly List<int> _indexes = new List<int>();
        public IReadOnlyList<int> Indexes => _indexes;

        public int Count => Indexes.Count;
        public TItem this[int index] => Wrapped[Indexes[index]];

        readonly IAsyncLock _lock;
        public IAsyncLock Lock => _lock;

        #region Constructors

        //  IMPLEMENTATION:
        //      By default don't use a lock, but allow custom lock to be provided

        public IndexFilter(IReadOnlyList<TItem> wrapped)
        : this(new NoLockAsyncLock(), wrapped, null)
        { }

        public IndexFilter(IReadOnlyList<TItem> wrapped, params int[] indexes)
            : this(new NoLockAsyncLock(), wrapped, indexes)
        { }

        public IndexFilter(IReadOnlyList<TItem> wrapped, IReadOnlyList<int> indexes)
            : this(new NoLockAsyncLock(), wrapped, indexes)
        { }

        public IndexFilter(IAsyncLock syncLock, IReadOnlyList<TItem> wrapped, IReadOnlyList<int> indexes)
        {
            _lock = syncLock;
            Wrapped = wrapped;

            if (indexes == null)
            {
                // include all indexes
                _indexes.AddRange(Enumerable.Range(0, wrapped.Count));
            }
            else
            {
                _indexes.AddRange(indexes);
            }
        }

        #endregion

        public IEnumerator<TItem> GetEnumerator()
        {
            for (int i = 0; i < Indexes.Count; i++)
            {
                yield return Wrapped[Indexes[i]];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddIndex(int index)
        {
            using (Lock.AcquireWriteLock())
            {
                _indexes.Add(index);
            }
        }

        public bool TryRemoveIndex(int index)
        {
            using (Lock.AcquireWriteLock())
            {
                return _indexes.Remove(index);
            }
        }

        public void Reset(IReadOnlyList<int> newIndexes)
        {
            using (Lock.AcquireWriteLock())
            {
                _indexes.Clear();
                _indexes.AddRange(newIndexes);
            }
        }

        public string DebuggerDisplay => $"Filtered {Count} out of {Wrapped.Count}";
    }
}
