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

        public bool IsWriteLockHeld => Environment.CurrentManagedThreadId == _writeOwnerThreadId;

        public bool IsReadLockHeld => _currentState > 0;

        readonly HashSet<_Waiter> _waitingReaders = new HashSet<_Waiter>();
        readonly Queue<_Waiter> _waitingWriters = new Queue<_Waiter>();

        class _Waiter
        {
            public readonly TaskCompletionSource<ILockAcquisition> TaskCompletionSource;
            public readonly TimeoutExpiry Timeout;
            public readonly CancellationToken CancellationToken;

            public _Waiter(TaskCompletionSource<ILockAcquisition> taskCompletionSource, int millisecondsTimeout, CancellationToken ct)
            {
                TaskCompletionSource = taskCompletionSource;
                Timeout = new TimeoutExpiry(millisecondsTimeout);
                CancellationToken = ct;
            }
        }

        readonly IAsyncLock InternalLock;

        // -1 : Write Lock
        // 0  : No Lock
        // 1+ : number of current writers
        volatile int _currentState = STATE_NOLOCK;

        const int STATE_NOLOCK = 0;
        const int STATE_WRITELOCK = -1;

        const int NO_THREAD = -1;

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

        void ReleaseReadLock(int ownerThreadId)
        {
            using (var a = InternalLock.AcquireWriteLock())
            {
                _readOwnerThreadIds.Remove(ownerThreadId);

                _currentState--;

                if (_currentState == STATE_NOLOCK)
                {
                    // there are no more read locks

                    if (_waitingWriters.Count == 0)
                    {
                        // there are no waiting writers
                        // nothing to do here
                        return;
                    }
                    else
                    {
                        // there are waiting writers
                        // grant lock to next in queue

                        _currentState = STATE_WRITELOCK;
                        bool ok = false;

                        while (_waitingWriters.Count > 0)
                        {
                            var next_writer_waiter = _waitingWriters.Dequeue();

                            if (next_writer_waiter.CancellationToken.IsCancellationRequested)
                                continue;

                            if (next_writer_waiter.Timeout.HasTimedOut)
                                continue;

                            var write_lock = AcquireWriteLock(new SyncOptions(next_writer_waiter.Timeout.MillisecondsLeft, next_writer_waiter.CancellationToken));

                            // thaw waiting task
                            if(next_writer_waiter.TaskCompletionSource.TrySetResult(write_lock))
                            {
                                ok = true;
                                break;
                            }
                            else
                            {
                                write_lock.Dispose();
                            }
                        }

                        // no more waiting writers
                        // set lock state to no lock

                        if (!ok)
                            _currentState = STATE_NOLOCK;
                    }
                }
            }
        }

        void ReleaseWriteLock()
        {
            using (InternalLock.AcquireWriteLock())
            {
                _writeOwnerThreadId = NO_THREAD;
            }
        }
    }
}
