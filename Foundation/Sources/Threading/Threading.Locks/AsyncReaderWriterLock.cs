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

            return new MonitorAcqusition(this, new CorrelationToken("__automatic_sync__"));
        }

        public void Release(ICorrelationToken correlationToken)
        {
            Monitor.Exit(Sync);
        }

        class MonitorAcqusition : DisposableObject, ILockAcquisition
        {
            SyncLock Owner;

            public ICorrelationToken CorrelationToken { get; private set; }

            public MonitorAcqusition(SyncLock owner, ICorrelationToken correlationToken)
            {
                Owner = owner ?? throw new ArgumentNullException(nameof(owner));
                CorrelationToken = correlationToken ?? throw new ArgumentNullException(nameof(correlationToken));
            }

            public bool IsLockHeld => true;

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                Owner.Release(CorrelationToken);
            }
        }
    }

    public partial class AsyncReaderWriterLock : IAsyncLock, IAsyncReadLock
    {
        readonly _CompositeAsyncLock CompositeLock;

        LockRecursionPolicy _recursionPolicy = LockRecursionPolicy.NoRecursion;
        public LockRecursionPolicy RecursionPolicy => _recursionPolicy;

        #region Owners

        _LockOwnership _readOwner;

        public IReadOnlyList<LockOwnership> ReadOwners
        {
            get
            {
                using (InternalLock.AcquireWriteLock())
                {
                    var ro = _readOwner;
                    return ro?.CorrelationTokenToAcquisitionCount.Select(x => new LockOwnership(x.Key, x.Value)).ToArray();
                }
            }
        }

        volatile _LockOwnership _writeOwner;
        public LockOwnership WriteOwner => _writeOwner?.CorrelationTokenToAcquisitionCount.Select(x => new LockOwnership(x.Key, x.Value)).FirstOrDefault();

        public IReadOnlyList<LockOwnership> AllOwners
        {
            get
            {
                using (InternalLock.AcquireWriteLock())
                {
                    return
                    _writeOwner?.CorrelationTokenToAcquisitionCount.Select(x => new LockOwnership(x.Key, x.Value))
                    .Concat(_readOwner?.CorrelationTokenToAcquisitionCount.Select(x => new LockOwnership(x.Key, x.Value)))
                    .ToArray();
                }
            }
        }

        #endregion

        public long LockId { get; } = _LockIdProvider.GetNextId();
        public string Name { get; private set; }

        readonly Queue<_Waiter> _waitingReaders = new Queue<_Waiter>();
        readonly Queue<_Waiter> _waitingWriters = new Queue<_Waiter>();

        readonly SyncLock InternalLock;

        public LockState State
        {
            get
            {
                if (_currentState == STATE_NOLOCK)
                    return LockState.NoLock;
                else if (_currentState == STATE_WRITELOCK)
                    return LockState.Write;
                else
                    return LockState.Read;
            }
        }

        // -1 : Write Lock
        // 0  : No Lock
        // 1+ : number of current writers
        volatile int _currentState = STATE_NOLOCK;

        const int STATE_NOLOCK = 0;
        const int STATE_WRITELOCK = -1;
        
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
        /// True if current lock acquisition attempt is recursive, false otherwise.
        /// </summary>
        /// <returns></returns>
        bool _IsLockAcquisitionAttemptRecursive__NOLOCK(LockType lockType, ICorrelationToken correlationToken)
        {
            switch (lockType)
            {
                case LockType.Read:
                    if (_readOwner == null)
                        return false;
                    return _readOwner.ContainsAcquisition(correlationToken);// todo: should this allow in when write held and instead auto-upgrade to write?
                case LockType.Write:
                    if (_writeOwner == null)
                        return false;
                    return _writeOwner.ContainsAcquisition(correlationToken);
                case LockType.UpgradeableRead:
                    throw new NotSupportedException(lockType.ToString());
            }

            return false;
        }

        void _AddNextReader__NOLOCK(ICorrelationToken correlationToken)
        {
            if (_readOwner == null) throw new InvalidOperationException($"Lock does not have any readers. Use {nameof(_AddFirstReader__NOLOCK)} instead.");

            _readOwner.AddAcquisition(correlationToken);

            _currentState++;
        }

        void _AddFirstReader__NOLOCK(ICorrelationToken correlationToken, IDisposable disposeWhenReleased)
        {
            if(_readOwner != null) throw new InvalidOperationException($"Lock already has a reader. Use {nameof(_AddNextReader__NOLOCK)} instead.");

            _readOwner = new _LockOwnership(disposeWhenReleased);
            _readOwner.AddAcquisition(correlationToken);

            _currentState++;
        }

        void ReleaseReadLock(ICorrelationToken correlationToken)
        {
            using (var l = InternalLock.AcquireWriteLock())
            {
                if (_readOwner == null) throw new InvalidOperationException($"Attempt to release read lock which isn't held, {correlationToken.ToString()}");

                _readOwner.RemoveAcquisition(correlationToken);
                _currentState--;

                if (_currentState == STATE_NOLOCK)
                {
                    _readOwner.Dispose();
                    _readOwner = null;
                    AfterLockReleased_NOLOCK(l);
                }
            }
        }

        void _AddRecursiveWriter(ICorrelationToken correlationToken)
        {
            if (_writeOwner == null) throw new InvalidOperationException($"Attempt to add recursive writer but no writer held.");
            if (!_writeOwner.ContainsAcquisition(correlationToken)) throw new InvalidOperationException($"Attempt to add recursive writer but different writer held.");

            _writeOwner.AddAcquisition(correlationToken);
        }

        void _AddWriter(ICorrelationToken correlationToken, IDisposable disposeWhenReleased)
        {
            if (_writeOwner != null) throw new InvalidOperationException($"Lock already has a writer.");

            _writeOwner = new _LockOwnership(disposeWhenReleased);
            _writeOwner.AddAcquisition(correlationToken);

            _currentState = STATE_WRITELOCK;
        }

        void ReleaseWriteLock(ICorrelationToken correlationToken)
        {
            using (var l = InternalLock.AcquireWriteLock())
            {
                if (_writeOwner == null) throw new InvalidOperationException($"Attempt to release write lock which isn't held, {correlationToken.ToString()}");

                _writeOwner.RemoveAcquisition(correlationToken);

                if (_writeOwner.IsOwned)
                    return;

                _writeOwner.Dispose();
                _writeOwner = null;

                _currentState = STATE_NOLOCK;

                AfterLockReleased_NOLOCK(l);
            }
        }

        void ProcessQueuedLocks()
        {
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
                            lock_acquisition =
                            await _AcquireLockAsync__NOLOCK(
                                LockType.Write, 
                                new AsyncOptions(
                                    r.Timeout.MillisecondsLeft, 
                                    r.CancellationToken,
                                    r.CorrelationToken),
                                lockAcquisition);

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
                                lock_acquisition =
                                await
                                _AcquireLockAsync__NOLOCK(
                                    LockType.Read,
                                    new AsyncOptions(
                                        r.Timeout.MillisecondsLeft, 
                                        r.CancellationToken,
                                        r.CorrelationToken),
                                    lockAcquisition);

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
                            catch (Exception ex)
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