using SquaredInfinity.Disposables;
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
        {
            var ao = AsyncOptions.Default;

            return 
                await 
                AcquireWriteLockAsync(ao)
                .ConfigureAwait(ao.ContinueOnCapturedContext);
        }

        public async Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options)
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

            using (var l = await InternalLock.AcquireWriteLockAsync(options).ConfigureAwait(options.ContinueOnCapturedContext))
            {
                if (!l.IsLockHeld)
                    return _FailedLockAcquisition.Instance;

                _readOwnerThreadIds.Add(System.Environment.CurrentManagedThreadId);

                // we are not in write lock and there is no pending writer
                // just acquire another read lock
                if (_currentState >= STATE_NOLOCK && _waitingWriter == null)
                {
                    if (CompositeLock == null)
                    {
                        // no children to lock, we can just grant reader-lock and return
                        _currentState++;
                        return new _ReadLockAcquisition(this, null);
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
                                new _ReadLockAcquisition(owner: this, disposeWhenDone: children_acquisition);
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
                    _waitingReaders.Add(x);

                    return await x.Task;
                }
            }
        }
    }
}
