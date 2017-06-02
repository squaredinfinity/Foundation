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
        async Task<ILockAcquisition> AcquireLockAsync(LockType lockType, AsyncOptions options)
        {
            options.CancellationToken.ThrowIfCancellationRequested();

            if (options.MillisecondsTimeout == 0)
                return _FailedLockAcquisition.Instance;

            if (RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                throw new NotSupportedException("Async acquisitions are not supported by recursive locks");

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

            var sw = Stopwatch.StartNew();

            var result = (Task<ILockAcquisition>)null;

            using (var l = InternalLock.AcquireWriteLock())
            {
                result = AcquireOrQueueLockAsync__NOLOCK(lockType, options, state_comparison_type, l);
            }

            return await result;
        }

        async Task<ILockAcquisition> AcquireOrQueueLockAsync__NOLOCK(LockType lockType, AsyncOptions options, ComparisonType stateComparisonType, ILockAcquisition internalLock)
        {
            if (StateComparer.Compare(_currentState, STATE_NOLOCK, stateComparisonType) && _waitingWriters.Count == 0)
            {
                return await AcquireLockAsync__NOLOCK(lockType, options, internalLock).ConfigureAwait(options.ContinueOnCapturedContext);
            }
            else
            {
                return await QueueLockAsync__NOLOCK(lockType, options, internalLock).ConfigureAwait(options.ContinueOnCapturedContext);
            }
        }

        async Task<ILockAcquisition> AcquireLockAsync__NOLOCK(LockType lockType, AsyncOptions options, ILockAcquisition internalLock)
        {
            // we are not in write or read lock and there is no waiting writer
            // just acquire another read lock

            if (CompositeLock == null || CompositeLock?.ChildrenCount == 0)
            {
                switch (lockType)
                {
                    case LockType.Read:
                        // no children to lock, we can just grant reader-lock and return
                        _readOwnerThreadIds.Add(Environment.CurrentManagedThreadId);
                        _currentState++;
                        return new _ReadLockAcquisition(this, Environment.CurrentManagedThreadId, null);
                    case LockType.Write:
                        // no children to lock, we can just grant writer-lock and return
                        _writeOwnerThreadId = Environment.CurrentManagedThreadId;
                        _currentState = STATE_WRITELOCK;
                        return new _WriteLockAcquisition(this, null);
                    default:
                        throw new NotSupportedException($"specified lock type is not supported, {lockType}");
                }
            }
            else
            {
                var provisional_acquisition = (ILockAcquisition)null;

                // there are children
                // grant lock and try to acquire children locks
                switch (lockType)
                {
                    case LockType.Read:
                        _currentState++;
                        provisional_acquisition = new _ReadLockAcquisition(this, NO_THREAD);
                        break;
                    case LockType.Write:
                        _currentState = STATE_WRITELOCK;
                        provisional_acquisition = new _WriteLockAcquisition(this);
                        break;
                    default:
                        throw new NotSupportedException($"specified lock type is not supported, {lockType}");
                }

                // release the internal lock while awaiting children
                // there's a potential of continuation on another thread
                // and we don't really need internal lock while waiting
                // as this state will no longer be modified
                internalLock.Dispose();

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

                    // release provisional acquisition
                    provisional_acquisition.Dispose();

                    //return failure
                    return new _FailedLockAcquisition();
                }

                using (var l2 = InternalLock.AcquireWriteLock())
                {
                    // replace provisional acquisition with actual acquisition with children and thread id
                    provisional_acquisition = null;

                    switch (lockType)
                    {
                        case LockType.Read:
                            _readOwnerThreadIds.Add(Environment.CurrentManagedThreadId);
                            return new _ReadLockAcquisition(this, Environment.CurrentManagedThreadId, children_acquisition);
                        case LockType.Write:
                            _writeOwnerThreadId = Environment.CurrentManagedThreadId;
                            return new _WriteLockAcquisition(this, children_acquisition);
                        default:
                            throw new NotSupportedException($"specified lock type is not supported, {lockType}");
                    }
                }
            }
        }

        async Task<ILockAcquisition> QueueLockAsync__NOLOCK(LockType lockType, AsyncOptions options, ILockAcquisition internalLock)
        {
            var completion_source = new TaskCompletionSource<ILockAcquisition>();
            completion_source.TimeoutAfter(options.MillisecondsTimeout, _ => _.SetResult(_FailedLockAcquisition.Instance));

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
                    NO_THREAD, // set owner thread id to NO_THREAD. for async waits it will be assigned when async wait completes
                    options.MillisecondsTimeout,
                    options.CancellationToken));

            internalLock.Dispose();

            return await completion_source.Task.ConfigureAwait(options.ContinueOnCapturedContext);
        }
    }
}
