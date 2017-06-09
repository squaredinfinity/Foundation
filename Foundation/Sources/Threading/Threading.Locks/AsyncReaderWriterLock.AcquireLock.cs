    using SquaredInfinity.Comparers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public partial class AsyncReaderWriterLock
    {
        static readonly IComparerEx<int> StateComparer = new ComparerEx<int>();

        public ILockAcquisition AcquireLock(LockType lockType) => AcquireLock(lockType, SyncOptions.Default);
        public ILockAcquisition AcquireLock(LockType lockType, SyncOptions options)
        {
            options.CancellationToken.ThrowIfCancellationRequested();

            if (lockType != LockType.Read && lockType != LockType.Write)
                throw new NotSupportedException($"specified lock type is not supported, {lockType}");

            if (options.MillisecondsTimeout == 0)
                return new _FailedLockAcquisition(options.CorrelationToken);

            if (_IsLockAcquisitionAttemptRecursive__NOLOCK(lockType, options.CorrelationToken))
                return new _DummyLockAcquisition(options.CorrelationToken);

            if (options.CorrelationToken == null)
                options = options.CorrelateThisThread();

            using (var l = InternalLock.AcquireWriteLock())
            {
                return AcquireOrQueueLock_NOLOCK(lockType, l, options);
            }
        }

        ILockAcquisition AcquireOrQueueLock_NOLOCK(
            LockType lockType,
            ILockAcquisition internalLockAcquisition,
            SyncOptions options)
        {
            if (options.CorrelationToken == null)
                throw new InvalidOperationException("Correlation Token must be set");

            if (options.MillisecondsTimeout == 0)
                return new _FailedLockAcquisition(options.CorrelationToken);

            if (_IsLockAcquisitionAttemptRecursive__NOLOCK(lockType, options.CorrelationToken))
            {
                if (RecursionPolicy == LockRecursionPolicy.NoRecursion)
                    throw new LockRecursionException();

                switch (lockType)
                {
                    case LockType.Read:
                        _AddNextReader__NOLOCK(options.CorrelationToken);
                        return new _LockAcquisition(this, LockType.Read, options.CorrelationToken);
                    case LockType.Write:
                        _AddRecursiveWriter(options.CorrelationToken);
                        return new _LockAcquisition(this, LockType.Write, options.CorrelationToken);
                    case LockType.UpgradeableRead:
                    default:
                        throw new NotSupportedException(lockType.ToString());
                }
            }

            switch (lockType)
            {
                case LockType.Read:
                    // for reads, state must be NO LOCK (0) or READ (1+)

                    // if 0, no lock currently held so acquire full lock on this and any children
                    if (_currentState == 0)
                    {
                        return _AcquireLock_NOLOCK(lockType, options);
                    }
                    // if 1+, no need to acquire child locks as they are already held by previous acquisition
                    else if (_currentState >= 1)
                    {
                        _AddNextReader__NOLOCK(options.CorrelationToken);
                        return new _LockAcquisition(this, LockType.Read, options.CorrelationToken);
                    }
                    // write lock held, queue this lock for later
                    else
                    {
                        return QueueLock_NOLOCK(lockType, internalLockAcquisition, options);
                    }
                case LockType.Write:
                    // for writes, state must be NO LOCK (0)
                    if(_currentState == 0)
                    {
                        return _AcquireLock_NOLOCK(lockType, options);
                    }
                    else
                    {
                        return QueueLock_NOLOCK(lockType, internalLockAcquisition, options);
                    }
                default:
                    throw new NotSupportedException(lockType.ToString());
            }
        }

        ILockAcquisition QueueLock_NOLOCK(LockType lockType, ILockAcquisition internalLockAcquisition, SyncOptions options)
        {
            var completion_source = new TaskCompletionSource<ILockAcquisition>();

            var waiter_queue = (Queue<_Waiter>)null;

            if (lockType == LockType.Read)
            {
                // we are in write mode
                // or there is a pending writer waiting
                // add this reader to waiting readers list

                waiter_queue = _waitingReaders;
            }
            else if (lockType == LockType.Write)
            {
                // we are in read mode
                // or there is a pending writer waiting
                // add this writer to waiting writers list

                waiter_queue = _waitingWriters;
            }
            else
            {
                throw new NotSupportedException($"specified lock type is not supported, {lockType}");
            }

            waiter_queue.Enqueue(
                new _Waiter(
                    completion_source,
                    options.CorrelationToken,
                    options.MillisecondsTimeout,
                    options.CancellationToken));

            // unlock this instance
            internalLockAcquisition.Dispose();

            // block this thread until task completes
            if (completion_source.Task.Wait(options.MillisecondsTimeout, options.CancellationToken))
                return completion_source.Task.Result;
            else
                return new _FailedLockAcquisition(options.CorrelationToken);
        }

        ILockAcquisition _AcquireLock_NOLOCK(LockType lockType, SyncOptions options)
        {
            // we are not in write or read lock and there is no waiting writer
            // just acquire another lock
            
            // todo this shouldn't acquire read on children every time
            // only on a first lock acquisition
            // then removed when last acquisition is released
            if (CompositeLock == null || CompositeLock.ChildrenCount == 0)
            {
                if (lockType == LockType.Read)
                {
                    // no children to lock, we can just grant reader-lock and return
                    _AddFirstReader__NOLOCK(options.CorrelationToken, null);
                    return new _LockAcquisition(this, LockType.Read, options.CorrelationToken);
                }
                else if (lockType == LockType.Write)
                {
                    // no children to lock, we can just grant writer-lock and return
                    _AddWriter(options.CorrelationToken, null);
                    return new _LockAcquisition(this, LockType.Write, options.CorrelationToken);
                }
                else
                {
                    throw new NotSupportedException($"specified lock type is not supported, {lockType}");
                }
            }
            else
            {
                // there might be children
                // try locking them now

                // try to lock children
                var children_acquisition =
                    CompositeLock
                    .LockChildren(lockType, options);

                if (!children_acquisition.IsLockHeld)
                {
                    // couldn't acquire children, return failure
                    children_acquisition.Dispose();
                    return new _FailedLockAcquisition(options.CorrelationToken);
                }

                if (lockType == LockType.Read)
                {
                    _AddFirstReader__NOLOCK(options.CorrelationToken, children_acquisition);
                    return new _LockAcquisition(this, LockType.Read, options.CorrelationToken);
                }
                else if (lockType == LockType.Write)
                {
                    _AddWriter(options.CorrelationToken, children_acquisition);
                    return new _LockAcquisition(this, LockType.Write, options.CorrelationToken);
                }
                else
                {
                    throw new NotSupportedException($"specified lock type is not supported, {lockType}");
                }
            }
        }
    }
}
