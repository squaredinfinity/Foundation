using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public partial class AsyncReaderWriterLock : IAsyncLock, IAsyncReadLock
    {
        readonly _CompositeAsyncLock CompositeLock;

        LockRecursionPolicy _recursionPolicy = LockRecursionPolicy.NoRecursion;
        public LockRecursionPolicy RecursionPolicy => _recursionPolicy;

        HashSet<int> _readOwnerThreadIds = new HashSet<int>();
        public IReadOnlyList<int> ReadOwnerThreadIds => _readOwnerThreadIds.ToArray();

        volatile int _writeOwnerThreadId = -1;
        public int WriteOwnerThreadId => _writeOwnerThreadId;

        public long LockId { get; } = _LockIdProvider.GetNextId();
        public string Name { get; private set; }

        public bool IsWriteLockHeld => System.Environment.CurrentManagedThreadId == _writeOwnerThreadId;

        public bool IsReadLockHeld => throw new NotImplementedException();

        readonly HashSet<TaskCompletionSource<ILockAcquisition>> _waitingReaders = new HashSet<TaskCompletionSource<ILockAcquisition>>();
        volatile TaskCompletionSource<ILockAcquisition> _waitingWriter;

        readonly IAsyncLock InternalLock;

        // -1 : Write Lock
        // 0  : No Lock
        // 1+ : number of current writers
        volatile int _currentState = STATE_NOLOCK;

        const int STATE_NOLOCK = 0;
        const int STATE_WRITELOCK = -1;

        public AsyncReaderWriterLock(
            string name = "",
            LockRecursionPolicy recursionPolicy = LockRecursionPolicy.SupportsRecursion,
            bool supportsComposition = true)
        {
            Name = name;
            _recursionPolicy = recursionPolicy;

            if (supportsComposition)
                CompositeLock = new _CompositeAsyncLock();
            else
                CompositeLock = null;

            // there's no need for internal lock to support recursion (implementation of this type will avoid this need)
            // there's no need for internal lock to support composition (no child locks will be added)
            InternalLock = new AsyncLock(recursionPolicy: LockRecursionPolicy.NoRecursion, supportsComposition: false);
        }
    }
}
