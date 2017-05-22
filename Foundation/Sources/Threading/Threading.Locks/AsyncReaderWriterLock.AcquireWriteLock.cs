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
        public ILockAcquisition AcquireWriteLock()
        {
            var so = SyncOptions.Default;
            return AcquireWriteLock(so);
        }

        public ILockAcquisition AcquireWriteLock(SyncOptions options)
        {
            if (RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
            {
                if (ReadOwnerThreadIds.Contains(Environment.CurrentManagedThreadId))
                {
                    // lock support re-entrancy and is already owned by this thread
                    // just return
                    return new _DummyLockAcquisition();
                }
            }
            else if (ReadOwnerThreadIds.Contains(Environment.CurrentManagedThreadId))
            {
                // cannot acquire non-recursive lock from thread which already owns it.
                throw new LockRecursionException();
            }


            using (var l = InternalLock.AcquireWriteLock(options))
            {
                if (!l.IsLockHeld)
                    return _FailedLockAcquisition.Instance;

                var ownerThreadId = Environment.CurrentManagedThreadId;

                if (_currentState == STATE_NOLOCK && _waitingWriters.Count == 0)
                {
                    _writeOwnerThreadId = ownerThreadId;

                    // we are not in write or read lock and there is no waiting writer
                    // just acquire another read lock

                    if (CompositeLock == null)
                    {
                        // no children to lock, we can just grant writer-lock and return
                        _currentState = STATE_WRITELOCK;
                        return new _WriteLockAcquisition(this, null);
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
                                .LockChildren(LockType.Write, options);

                            if (!children_acquisition.IsLockHeld)
                            {
                                // couldn't acquire children, return failure

                                children_acquisition.Dispose();
                                return new _FailedLockAcquisition();
                            }

                            _currentState = STATE_WRITELOCK;
                            return
                                new _WriteLockAcquisition(owner: this, disposeWhenDone: children_acquisition);
                        }
                        catch
                        {
                            _writeOwnerThreadId = NO_THREAD;
                            throw;
                        }
                    }
                }
                else
                {
                    // we are in read mode
                    // or there is a pending writer waiting
                    // add this writer to waiting writers list

                    var x = new TaskCompletionSource<ILockAcquisition>();
                    _waitingReaders.Add(new _Waiter(x, options.MillisecondsTimeout, options.CancellationToken));

                    // unlock this instance
                    l.Dispose();

                    // block this thread until task completes
                    if (x.Task.Wait(options.MillisecondsTimeout, options.CancellationToken))
                        return x.Task.Result;
                    else
                        return _FailedLockAcquisition.Instance;
                }
            }
        }
    }
}
