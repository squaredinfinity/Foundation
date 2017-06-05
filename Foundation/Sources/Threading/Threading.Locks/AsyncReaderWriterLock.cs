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

        #region Owners

        HashSet<LockOwnership> _readOwners = new HashSet<LockOwnership>();
        public IReadOnlyList<LockOwnership> ReadOwners
        {
            get
            {
                using (InternalLock.AcquireWriteLock())
                {
                    return _readOwners.ToArray();
                }
            }
        }

        volatile LockOwnership _writeOwner;
        public LockOwnership WriteOwner => _writeOwner;

        public IReadOnlyList<LockOwnership> AllOwners
        {
            get
            {
                using (InternalLock.AcquireWriteLock())
                {
                    if(_writeOwner != null)
                        return new[] { _writeOwner }.Concat(_readOwners).ToArray();
                    else
                        return _readOwners.ToArray();
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
        /// Throws if attempt is recursive but recursion isn't allowed.
        /// </summary>
        /// <returns></returns>
        bool _IsLockAcquisitionAttemptRecursive(LockType lockType, ICorrelationToken correlationToken)
        {
            if (RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
            {
                switch (lockType)
                {
                    case LockType.Read:
                        return ReadOwners.Select(x => x.CorrelationToken).Contains(correlationToken); // todo: should this allow in when write held and instead auto-upgrade to write?
                    case LockType.Write:
                        return WriteOwner?.CorrelationToken == correlationToken;
                    case LockType.UpgradeableRead:
                        throw new NotSupportedException(lockType.ToString());
                }
            }
            else 
            {
                return AllOwners.Select(x => x.CorrelationToken).Contains(correlationToken);
            }

            return false;
        }

        void _AddReader(ICorrelationToken correlationToken)
        {
            var existing =
                (from o in _readOwners
                 where o.CorrelationToken == correlationToken
                 select o).FirstOrDefault();

            if (existing == null)
            {
                _readOwners.Add(new LockOwnership(correlationToken));
                _currentState++;
            }
            else
            {
                existing.AcquisitionCount++;
            }
        }

        void _AddWriter(ICorrelationToken correlationToken)
        {
            if (_writeOwner == null)
                _writeOwner = new LockOwnership(correlationToken);
            else
                _writeOwner.AcquisitionCount++;

            _currentState = STATE_WRITELOCK;
        }

        void ReleaseReadLock(ICorrelationToken ownerCorrelationToken)
        {
            using (var l = InternalLock.AcquireWriteLock())
            {
                var existing =
                (from o in _readOwners
                 where o.CorrelationToken == ownerCorrelationToken
                 select o).FirstOrDefault();

                if(existing == null)
                {
                    throw new InvalidOperationException($"Attempt to release lock which isn't held, {ownerCorrelationToken.ToString()}");
                }
                else if(existing.AcquisitionCount > 1)
                {
                    existing.AcquisitionCount--;
                    return;
                }
                else
                {
                    // change state as soon as possible
                    _readOwners.Remove(existing);
                    _currentState--;
                }

                if(_currentState == STATE_NOLOCK)
                    AfterLockReleased_NOLOCK(l);
            }
        }

        void ReleaseWriteLock()
        {
            using (var l = InternalLock.AcquireWriteLock())
            {
                _writeOwner.AcquisitionCount--;

                if (_writeOwner.AcquisitionCount == 0)
                {
                    _currentState = STATE_NOLOCK;
                    
                    AfterLockReleased_NOLOCK(l);
                }
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
                            await AcquireLockAsync__NOLOCK(
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
                                AcquireLockAsync__NOLOCK(
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