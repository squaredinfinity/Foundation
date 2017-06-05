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
                return _FailedLockAcquisition.Instance;

            if (_IsLockAcquisitionAttemptRecursive(lockType, options.CorrelationToken))
                return new _DummyLockAcquisition();

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
                return _FailedLockAcquisition.Instance;

            if (_IsLockAcquisitionAttemptRecursive(lockType, options.CorrelationToken))
            {
                // increment lock acquisition count
                return new _DummyLockAcquisition();
            }

            var state_comparison_type = ComparisonType.Equal;

            if (lockType == LockType.Read)
            {
                // for reads, state must be NO LOCK (0) or READ (1+)
                state_comparison_type = ComparisonType.GreaterOrEqual;
            }
            else if (lockType == LockType.Write)
            {
                // for writes, state must be NO LOCK (0)
                state_comparison_type = ComparisonType.Equal;
            }
            else
            {
                throw new NotSupportedException($"specified lock type is not supported, {lockType}");
            }

            if (StateComparer.Compare(_currentState, STATE_NOLOCK, state_comparison_type) && _waitingWriters.Count == 0)
            {
                return AcquireLock_NOLOCK(lockType, options);
            }
            else
            {
                return QueueLock_NOLOCK(lockType, internalLockAcquisition, options);
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
                return _FailedLockAcquisition.Instance;
        }

        ILockAcquisition AcquireLock_NOLOCK(LockType lockType, SyncOptions options)
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
                    _AddReader(options.CorrelationToken);
                    return new _ReadLockAcquisition(this, options.CorrelationToken, null);
                }
                else if (lockType == LockType.Write)
                {
                    // no children to lock, we can just grant writer-lock and return
                    _AddWriter(options.CorrelationToken);
                    return new _WriteLockAcquisition(this, null);
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
                    return new _FailedLockAcquisition();
                }

                if (lockType == LockType.Read)
                {
                    _AddReader(options.CorrelationToken);
                    return
                        new _ReadLockAcquisition(owner: this,correlationToken: options.CorrelationToken , disposeWhenDone: children_acquisition);
                }
                else if (lockType == LockType.Write)
                {
                    _AddWriter(options.CorrelationToken);
                    return
                        new _WriteLockAcquisition(owner: this, disposeWhenDone: children_acquisition);
                }
                else
                {
                    throw new NotSupportedException($"specified lock type is not supported, {lockType}");
                }
            }
        }
    }
}
