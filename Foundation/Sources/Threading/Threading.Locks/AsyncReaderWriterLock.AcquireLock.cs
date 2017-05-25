using SquaredInfinity.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public partial class AsyncReaderWriterLock
    {
        static readonly IComparerEx<int> StateComparer = new ComparerEx<int>();

        ILockAcquisition AcquireLock(LockType lockType, SyncOptions options)
        {
            if (lockType != LockType.Read && lockType != LockType.Write)
                throw new NotSupportedException($"specified lock type is not supported, {lockType}");

            if (options.MillisecondsTimeout == 0)
                return _FailedLockAcquisition.Instance;

            if (IsLockAcquisitionRecursive())
                return new _DummyLockAcquisition();

            using (var l = InternalLock.AcquireWriteLock(options))
            {
                if (!l.IsLockHeld)
                    return _FailedLockAcquisition.Instance;

                return AcquireLock_NOLOCK(lockType, l, Environment.CurrentManagedThreadId, options);
            }
        }

        ILockAcquisition AcquireLock_NOLOCK(
            LockType lockType, 
            ILockAcquisition internalLockAcquisition, 
            int ownerThreadId, 
            SyncOptions options)
        {
            if (options.MillisecondsTimeout == 0)
                return _FailedLockAcquisition.Instance;

            if (IsLockAcquisitionRecursive())
                return new _DummyLockAcquisition();

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

            if(StateComparer.Compare(_currentState, STATE_NOLOCK, state_comparison_type) && _waitingWriters.Count == 0)
            {
                // we are not in write or read lock and there is no waiting writer
                // just acquire another read lock

                if (CompositeLock == null)
                {
                    if (lockType == LockType.Read)
                    {
                        // no children to lock, we can just grant reader-lock and return
                        _readOwnerThreadIds.Add(ownerThreadId);
                        _currentState++;
                        return new _ReadLockAcquisition(this, ownerThreadId, null);
                    }                    
                    else if(lockType == LockType.Write)
                    {
                        // no children to lock, we can just grant writer-lock and return
                        _writeOwnerThreadId = ownerThreadId;
                        _currentState = STATE_WRITELOCK;
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

                    try
                    {
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
                            _readOwnerThreadIds.Add(ownerThreadId);
                            _currentState++;
                            return
                                new _ReadLockAcquisition(owner: this, ownerThreadId: ownerThreadId, disposeWhenDone: children_acquisition);
                        }
                        else if(lockType == LockType.Write)
                        {
                            _writeOwnerThreadId = ownerThreadId;
                            _currentState = STATE_WRITELOCK;
                            return
                                new _WriteLockAcquisition(owner: this, disposeWhenDone: children_acquisition);
                        }
                        else
                        {
                            throw new NotSupportedException($"specified lock type is not supported, {lockType}");
                        }
                    }
                    catch
                    {
                        if (lockType == LockType.Read)
                        {
                            _readOwnerThreadIds.Remove(Environment.CurrentManagedThreadId);
                            throw;
                        }
                        else 
                        if(lockType == LockType.Write)
                        {
                            _writeOwnerThreadId = NO_THREAD;
                            throw;
                        }
                        else
                        {
                            throw new NotSupportedException($"specified lock type is not supported, {lockType}");
                        }
                    }
                }
            }
            else
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
                        Environment.CurrentManagedThreadId,
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
        }
    }
}
