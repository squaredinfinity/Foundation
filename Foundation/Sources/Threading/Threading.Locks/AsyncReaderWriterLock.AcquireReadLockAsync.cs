﻿using SquaredInfinity.Disposables;
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
        public async Task<ILockAcquisition> AcquireReadLockAsync()
            => await  AcquireReadLockAsync(AsyncOptions.Default).ConfigureAwait(AsyncOptions.Default.ContinueOnCapturedContext);

        public async Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options)
        {
            if (options.MillisecondsTimeout == 0)
                return _FailedLockAcquisition.Instance;

            if (IsLockAcquisitionRecursive())
                return new _DummyLockAcquisition();

            using (var l = await InternalLock.AcquireWriteLockAsync(options).ConfigureAwait(options.ContinueOnCapturedContext))
            {
                if (!l.IsLockHeld)
                    return _FailedLockAcquisition.Instance;

                var ownerThreadId = Environment.CurrentManagedThreadId;

                // we are not in write lock and there is no pending writer
                // just acquire another read lock
                if (_currentState >= STATE_NOLOCK && _waitingWriters.Count == 0)
                {
					_readOwnerThreadIds.Add(ownerThreadId);

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
                                await 
                                CompositeLock
                                .LockChildrenAsync(LockType.Read, options)
                                .ConfigureAwait(options.ContinueOnCapturedContext);

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

                    return await x.Task;
                }
            }
        }
    }
}
