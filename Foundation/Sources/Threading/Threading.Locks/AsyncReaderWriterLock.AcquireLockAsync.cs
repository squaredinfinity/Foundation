using SquaredInfinity.Comparers;
using SquaredInfinity.Extensions;
using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SquaredInfinity.Threading.Locks
{
    public partial class AsyncReaderWriterLock
    {
        public async Task<ILockAcquisition> AcquireLockAsync(LockType lockType)
            => await AcquireLockAsync(lockType, AsyncOptions.Default);
        public async Task<ILockAcquisition> AcquireLockAsync(LockType lockType, ICorrelationToken correlationToken) 
            => await AcquireLockAsync(lockType, AsyncOptions.Default.Correlate(correlationToken ?? throw new ArgumentNullException(nameof(correlationToken))));

        public async Task<ILockAcquisition> AcquireLockAsync(LockType lockType, AsyncOptions options)
        {
            options.CancellationToken.ThrowIfCancellationRequested();

            if (options.CorrelationToken == null)
                options = options.Correlate(new CorrelationToken("__automatic_async__"));

            if (options.MillisecondsTimeout == 0)
                return new _FailedLockAcquisition(options.CorrelationToken);

            var state_comparison_type = ComparisonType.Equal;

            switch (lockType)
            {
                case LockType.Read:
                    // for reads, state must be NO LOCK (0) or READ (1+)
                    state_comparison_type = ComparisonType.GreaterOrEqual;
                    break;
                case LockType.Write:
                    // for writes, state must be NO LOCK (0)
                    state_comparison_type = ComparisonType.Equal;
                    break;
                default:
                    throw new NotSupportedException($"specified lock type is not supported, {lockType}");
            }
            
            var result = (Task<ILockAcquisition>)null;

            using (var l = InternalLock.AcquireWriteLock())
            {
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

                result = AcquireOrQueueLockAsync__NOLOCK(lockType, options, state_comparison_type, l);
            }

            return await result;
        }

        async Task<ILockAcquisition> AcquireOrQueueLockAsync__NOLOCK(LockType lockType, AsyncOptions options, ComparisonType stateComparisonType, ILockAcquisition internalLock)
        {
            if (StateComparer.Compare(_currentState, STATE_NOLOCK, stateComparisonType) && _waitingWriters.Count == 0)
            {
                return await _AcquireLockAsync__NOLOCK(lockType, options, internalLock).ConfigureAwait(options.ContinueOnCapturedContext);
            }
            else
            {
                return await QueueLockAsync__NOLOCK(lockType, options, internalLock).ConfigureAwait(options.ContinueOnCapturedContext);
            }
        }

        async Task<ILockAcquisition> _AcquireLockAsync__NOLOCK(LockType lockType, AsyncOptions options, ILockAcquisition internalLock)
        {
            // we are not in write or read lock and there is no waiting writer
            // just acquire another read lock

            if (CompositeLock == null || CompositeLock?.ChildrenCount == 0)
            {
                switch (lockType)
                {
                    case LockType.Read:
                        // no children to lock, we can just grant reader-lock and return
                        _AddFirstReader__NOLOCK(options.CorrelationToken, null);
                        return new _LockAcquisition(this, LockType.Read, options.CorrelationToken);
                    case LockType.Write:
                        // no children to lock, we can just grant writer-lock and return
                        _AddWriter(options.CorrelationToken, null);
                        return new _LockAcquisition(this, LockType.Write, options.CorrelationToken);
                    default:
                        throw new NotSupportedException($"specified lock type is not supported, {lockType}");
                }
            }
            else
            {
                try
                {
                    var children_acquisition =
                        await
                        CompositeLock
                        .LockChildrenAsync(lockType, options)
                        .ConfigureAwait(options.ContinueOnCapturedContext);

                    if (!children_acquisition.IsLockHeld)
                    {
                        //# couldn't acquire children

                        // release locks on any children that might have been acquired
                        children_acquisition.Dispose();

                        //return failure
                        return new _FailedLockAcquisition(options.CorrelationToken);
                    }

                    using (var l2 = InternalLock.AcquireWriteLock())
                    {
                        switch (lockType)
                        {
                            case LockType.Read:
                                _AddFirstReader__NOLOCK(options.CorrelationToken, children_acquisition);
                                return new _LockAcquisition(this, LockType.Read, options.CorrelationToken);
                            case LockType.Write:
                                _AddWriter(options.CorrelationToken, children_acquisition);
                                return new _LockAcquisition(this, LockType.Write, options.CorrelationToken);
                            default:
                                throw new NotSupportedException($"specified lock type is not supported, {lockType}");
                        }
                    }
                }
                finally
                {
                    internalLock.Dispose();
                }
            }
        }

        async Task<ILockAcquisition> QueueLockAsync__NOLOCK(LockType lockType, AsyncOptions options, ILockAcquisition internalLock)
        {
            var completion_source = new TaskCompletionSource<ILockAcquisition>();
            completion_source.TimeoutAfter(options.MillisecondsTimeout, _ => _.SetResult(new _FailedLockAcquisition(options.CorrelationToken)));

            var waiter_queue = (Queue<_Waiter>)null;

            switch (lockType)
            {
                case LockType.Read:
                    // we are in write mode
                    // or there is a pending writer waiting
                    // add this reader to waiting readers list
                    waiter_queue = _waitingReaders;
                    break;
                case LockType.Write:
                    // we are in read mode
                    // or there is a pending writer waiting
                    // add this writer to waiting writers list
                    waiter_queue = _waitingWriters;
                    break;
                default:
                    throw new NotSupportedException($"specified lock type is not supported, {lockType}");
            }

            waiter_queue.Enqueue(
                new _Waiter(
                    completion_source,
                    options.CorrelationToken,
                    options.MillisecondsTimeout,
                    options.CancellationToken));

            internalLock.Dispose();

            return await completion_source.Task.ConfigureAwait(options.ContinueOnCapturedContext);
        }
    }
}
