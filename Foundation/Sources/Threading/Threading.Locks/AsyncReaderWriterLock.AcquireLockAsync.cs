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
            if (options.MillisecondsTimeout == 0)
                return _FailedLockAcquisition.Instance;

            if (IsLockAcquisitionRecursive())
                return new _DummyLockAcquisition();

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
            
            using(var l = InternalLock.AcquireWriteLock())
            {
                Debug.WriteLine("Internal Lock " + sw.ElapsedMilliseconds);

                if (StateComparer.Compare(_currentState, STATE_NOLOCK, state_comparison_type) && _waitingWriters.Count == 0)
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
                        // there might be children
                        // try locking them now

                        var children_acquisition =
                            await
                            CompositeLock
                            .LockChildrenAsync(lockType, options)
                            .ConfigureAwait(options.ContinueOnCapturedContext);

                        if (!children_acquisition.IsLockHeld)
                        {
                            // couldn't acquire children, return failure
                            children_acquisition.Dispose();
                            return new _FailedLockAcquisition();
                        }

                        if (lockType == LockType.Read)
                        {
                            _readOwnerThreadIds.Add(Environment.CurrentManagedThreadId);
                            _currentState++;
                            return new _ReadLockAcquisition(owner: this, ownerThreadId: Environment.CurrentManagedThreadId, disposeWhenDone: children_acquisition);
                        }
                        else if (lockType == LockType.Write)
                        {
                            _writeOwnerThreadId = Environment.CurrentManagedThreadId;
                            _currentState = STATE_WRITELOCK;
                            return new _WriteLockAcquisition(owner: this, disposeWhenDone: children_acquisition);
                        }
                        else
                        {
                            throw new NotSupportedException($"specified lock type is not supported, {lockType}");
                        }
                    }
                }
                else
                {
                    var completion_source = new TaskCompletionSource<ILockAcquisition>();
                    completion_source.TimeoutAfter(options.MillisecondsTimeout, _ => _.SetResult(_FailedLockAcquisition.Instance));

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
                            null, // set owner thread id to null. for async waits it will be assigned when async wait completes
                            options.MillisecondsTimeout, 
                            options.CancellationToken));

                    l.Dispose();

                    return await completion_source.Task;
                }
            }
        }
    }
}
