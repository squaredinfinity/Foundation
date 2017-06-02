using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    class SyncLock
    {
        readonly object Sync = new object();

        public ILockAcquisition AcquireWriteLock()
        {
            Monitor.Enter(Sync);

            return new MonitorAcqusition(this);
        }

        public void Release()
        {
            Monitor.Exit(Sync);
        }

        class MonitorAcqusition : DisposableObject, ILockAcquisition
        {
            SyncLock Owner;

            public MonitorAcqusition(SyncLock owner)
            {
                Owner = owner;
            }

            public bool IsLockHeld => true;

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                Owner.Release();
            }
        }
    }

    public partial class AsyncReaderWriterLock : IAsyncLock, IAsyncReadLock
    {
        readonly _CompositeAsyncLock CompositeLock;

        LockRecursionPolicy _recursionPolicy = LockRecursionPolicy.NoRecursion;
        public LockRecursionPolicy RecursionPolicy => _recursionPolicy;

        HashSet<int> _readOwnerThreadIds = new HashSet<int>();
        public IReadOnlyList<int> ReadOwnerThreadIds
        {
            get
            {
                using (InternalLock.AcquireWriteLock())
                {
                    return _readOwnerThreadIds.ToArray();
                }
            }
        }

        volatile int _writeOwnerThreadId = -1;
        public int WriteOwnerThreadId => _writeOwnerThreadId;

        public long LockId { get; } = _LockIdProvider.GetNextId();
        public string Name { get; private set; }

        /// <summary>
        /// True if current thread hold write lock
        /// </summary>
        public bool IsWriteLockHeld => Environment.CurrentManagedThreadId == _writeOwnerThreadId;

        /// <summary>
        /// True if current thread hold read lock
        /// </summary>
        public bool IsReadLockHeld => ReadOwnerThreadIds.Contains(Environment.CurrentManagedThreadId);

        readonly Queue<_Waiter> _waitingReaders = new Queue<_Waiter>();
        readonly Queue<_Waiter> _waitingWriters = new Queue<_Waiter>();

        readonly SyncLock InternalLock;

        public ReaderWriterLockState State
        {
            get
            {
                if (_currentState == STATE_NOLOCK)
                    return ReaderWriterLockState.NoLock;
                else if (_currentState == STATE_WRITELOCK)
                    return ReaderWriterLockState.Write;
                else
                    return ReaderWriterLockState.Read;
            }
        }

        // -1 : Write Lock
        // 0  : No Lock
        // 1+ : number of current writers
        volatile int _currentState = STATE_NOLOCK;

        const int STATE_NOLOCK = 0;
        const int STATE_WRITELOCK = -1;

        const int NO_THREAD = -1;

        public AsyncReaderWriterLock(LockRecursionPolicy recursionPolicy)
            : this("", recursionPolicy, supportsComposition: true)
        { }

        public AsyncReaderWriterLock(LockRecursionPolicy recursionPolicy, bool supportsComposition)
            : this("", recursionPolicy, supportsComposition)
        { }

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
            //InternalLock = new AsyncLock(recursionPolicy: LockRecursionPolicy.NoRecursion, supportsComposition: false);
            InternalLock = new SyncLock();
        }

        #region ICompositeAsyncLock

        public int ChildrenCount => CompositeLock.ChildrenCount;

        public void AddChild(IAsyncLock childLock)
        {
            if (CompositeLock == null)
                throw new NotSupportedException("This lock does not support .AddChild()");

            CompositeLock.AddChild(childLock);
        }

        public void AddChild(ILock childLock)
        {
            if (CompositeLock == null)
                throw new NotSupportedException("This lock does not support .AddChild()");

            CompositeLock.AddChild(childLock);
        }

        public void RemoveChild(IAsyncLock childLock)
        {
            if (CompositeLock == null)
                throw new NotSupportedException("This lock does not support .RemoveChild()");

            CompositeLock.RemoveChild(childLock);
        }

        public void RemoveChild(ILock childLock)
        {
            if (CompositeLock == null)
                throw new NotSupportedException("This lock does not support .RemoveChild()");

            CompositeLock.RemoveChild(childLock);
        }

        #endregion

        /// <summary>
        /// True if lock acquisition is recursive, false otherwise
        /// </summary>
        /// <returns></returns>
        bool IsLockAcquisitionRecursive()
        {
            if (RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
            {
                if (_writeOwnerThreadId == System.Environment.CurrentManagedThreadId)
                {
                    // lock support re-entrancy and is already owned by this thread
                    // just return
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (System.Environment.CurrentManagedThreadId == WriteOwnerThreadId)
            {
                // cannot acquire non-recursive lock from thread which already owns it.
                throw new LockRecursionException();
            }

            return false;
        }

        void ReleaseReadLock(int ownerThreadId)
        {
            using (InternalLock.AcquireWriteLock())
            {
                // change state as soon as possible
                _readOwnerThreadIds.Remove(ownerThreadId);
                _currentState--;

                ScheduleQueueProcessing();
            }
        }

        void ReleaseWriteLock()
        {
            using (var l = InternalLock.AcquireWriteLock())
            {
                _writeOwnerThreadId = NO_THREAD;
                _currentState = STATE_NOLOCK;

                //ScheduleQueueProcessing();
                AfterLockReleased_NOLOCK(l);
            }
        }

        void ScheduleQueueProcessing()
        {
            // start queued locks processing on another thread
            Task.Run(() => ProcessQueuedLocks());
            //ProcessQueuedLocks()
        }

        void ProcessQueuedLocks()
        {
            //using (var l = await InternalLock.AcquireWriteLockAsync())
            using (var l = InternalLock.AcquireWriteLock())
            {
                AfterLockReleased_NOLOCK(l);
            }
        }

        void AfterLockReleased_NOLOCK(ILockAcquisition lockAcquisition)
        {
            if (_currentState != STATE_NOLOCK)
            {
                // a lock is still in place (e.g. another reader)
                // nothing to do here
                return;
            }

            if (_waitingWriters.Count > 0)
            {
                // there are waiting writers
                // grant lock to next in queue
                while (_waitingWriters.Count > 0)
                {
                    var r = _waitingWriters.Dequeue();

                    if (r.CancellationToken.IsCancellationRequested)
                        continue;

                    if (r.Timeout.HasTimedOut)
                        continue;
                    
                    // acquire lock on different thread
                    Task.Run(async () =>
                    {
                        var lock_acquisition = (ILockAcquisition)null;

                        try
                        {
                            if (r.OwnerThreadId != NO_THREAD)
                            {
                                lock_acquisition =
                                AcquireLock_NOLOCK(LockType.Write, r.OwnerThreadId, new SyncOptions(r.Timeout.MillisecondsLeft, r.CancellationToken));
                            }
                            else
                            {
                                lock_acquisition =
                                await AcquireLockAsync__NOLOCK(LockType.Write, new AsyncOptions(r.Timeout.MillisecondsLeft, r.CancellationToken), lockAcquisition);
                            }

                            // Task Completion Source might have timed out, so check the result
                            if (r.TaskCompletionSource.TrySetResult(lock_acquisition))
                            {
                                // all ok
                            }
                            else
                            {
                                lock_acquisition.Dispose();
                                ProcessQueuedLocks();
                            }
                        }
                        catch(Exception ex)
                        {
                            r.TaskCompletionSource.TrySetException(ex);
                            lock_acquisition?.Dispose();
                            ProcessQueuedLocks();
                        }
                    });

                    // do not process queue any further
                    break;
                }
            }
            else
            {
                // there are no waiting writers
                // check if there are waiting readers

                if (_waitingReaders.Count == 0)
                {
                    // nothing to do, return
                    return;
                }
                else
                {
                    // let the readers in
                    while (_waitingReaders.Count > 0)
                    {
                        var r = _waitingReaders.Dequeue();

                        if (r.CancellationToken.IsCancellationRequested)
                            continue;

                        if (r.Timeout.HasTimedOut)
                            continue;
                        
                        Task.Run(async () =>
                        {
                            var lock_acquisition = (ILockAcquisition)null;

                            try
                            {
                                if (r.OwnerThreadId != NO_THREAD)
                                {
                                    lock_acquisition =
                                    AcquireLock_NOLOCK(LockType.Read, r.OwnerThreadId, new SyncOptions(r.Timeout.MillisecondsLeft, r.CancellationToken));
                                }
                                else
                                {
                                    lock_acquisition = 
                                    await 
                                    AcquireLockAsync__NOLOCK(LockType.Read, new AsyncOptions(r.Timeout.MillisecondsLeft, r.CancellationToken), lockAcquisition);
                                }

                                // Task Completion Source might have timed out, so check the result
                                if (r.TaskCompletionSource.TrySetResult(lock_acquisition))
                                {
                                    // all went ok, try to acquire next read lock
                                }
                                else
                                {
                                    lock_acquisition.Dispose();
                                    ProcessQueuedLocks();
                                }
                            }
                            catch(Exception ex)
                            {
                                r.TaskCompletionSource.TrySetException(ex);
                                lock_acquisition?.Dispose();
                                ProcessQueuedLocks();
                            }
                        });
                    }
                }
            }
        }
    }
}