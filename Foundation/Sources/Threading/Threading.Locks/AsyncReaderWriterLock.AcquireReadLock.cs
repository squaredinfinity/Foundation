﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public partial class AsyncReaderWriterLock
    {
        public ILockAcquisition AcquireReadLock()
        {
            var so = SyncOptions.Default;

            return AcquireReadLock(so);
        }

        public ILockAcquisition AcquireReadLock(SyncOptions options)
        {
            if (RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
            {
                if (ReadOwnerThreadIds.Contains(System.Environment.CurrentManagedThreadId))
                {
                    // lock support re-entrancy and is already owned by this thread
                    // just return
                    return new _DummyLockAcquisition();
                }
            }
            else if (ReadOwnerThreadIds.Contains(System.Environment.CurrentManagedThreadId))
            {
                // cannot acquire non-recursive lock from thread which already owns it.
                throw new LockRecursionException();
            }

            using (var l = InternalLock.AcquireWriteLock(options))
            {
                if (!l.IsLockHeld)
                    return _FailedLockAcquisition.Instance;

                var ownerThreadId = System.Environment.CurrentManagedThreadId;

                if (_currentState >= STATE_NOLOCK && _waitingWriters.Count == 0)
                {
                    _readOwnerThreadIds.Add(ownerThreadId);

                    // we are not in write lock and there is no waiting writer
                    // just acquire another read lock

                    if (CompositeLock == null)
                    {
                        // no children to lock, we can just grant reader-lock and return
                        _currentState++;
                        return new _ReadLockAcquisition(this, ownerThreadId, null);
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
                                .LockChildren(LockType.Read, options);

                            if (!children_acquisition.IsLockHeld)
                            {
                                // couldn't acquire children, return failure

                                children_acquisition.Dispose();
                                return new _FailedLockAcquisition();
                            }

                            return
                                new _ReadLockAcquisition(owner: this, ownerThreadId: ownerThreadId, disposeWhenDone: children_acquisition);
                        }
                        catch
                        {
                            _readOwnerThreadIds.Remove(Environment.CurrentManagedThreadId);
                            throw;
                        }
                    }
                }
                else
                {
                    // we are in write mode
                    // or there is a pending writer waiting
                    // add this reader to waiting readers list

                    var x = new TaskCompletionSource<ILockAcquisition>();
                    _waitingReaders.Add(new _Waiter(x, options.MillisecondsTimeout, options.CancellationToken));

                    if (x.Task.Wait(options.MillisecondsTimeout, options.CancellationToken))
                        return x.Task.Result;
                    else
                        return _FailedLockAcquisition.Instance;
                }
            }
        }
    }
}
