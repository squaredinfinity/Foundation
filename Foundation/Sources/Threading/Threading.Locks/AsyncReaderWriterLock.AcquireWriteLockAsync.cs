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
        public async Task<ILockAcquisition> AcquireWriteLockAsync()
            => await AcquireWriteLockAsync(AsyncOptions.Default);

        public async Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options)
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
                                await
                                CompositeLock
                                .LockChildrenAsync(LockType.Write, options)
                                .ConfigureAwait(options.ContinueOnCapturedContext);

                            if (!children_acquisition.IsLockHeld)
                            {
                                // couldn't acquire children, return failure

                                children_acquisition.Dispose();
                                return new _FailedLockAcquisition();
                            }

                            _currentState = STATE_WRITELOCK;
                            return new _WriteLockAcquisition(owner: this, disposeWhenDone: children_acquisition);
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
                    _waitingWriters.Enqueue(new _Waiter(x, options.MillisecondsTimeout, options.CancellationToken));

                    return await x.Task;
                }
            }
        }
    }
}
