using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Threading.Locks
{
    public partial class AsyncLock
    {
        #region Acquire Write Lock Async

        public async Task<ILockAcquisition> AcquireWriteLockAsync()
        {
            var ao = AsyncOptions.Default;

            return
                await
                AcquireWriteLockAsync(ao)
                .ConfigureAwait(ao.ContinueOnCapturedContext);
        }

        public async Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options)
        {
            if (RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
            {
                if (_writeOwnerThreadId == System.Environment.CurrentManagedThreadId)
                {
                    // lock support re-entrancy and is already owned by this thread
                    // just return
                    return new _DummyLockAcquisition();
                }
                else
                {
                    // nothing to do here
                    // just continue with normal execution flow
                }
            }
            else if (System.Environment.CurrentManagedThreadId == WriteOwnerThreadId)
            {
                // cannot acquire non-recursive lock from thread which already owns it.
                throw new LockRecursionException();
            }

            // lock parent first
            var ok =
                await
                InternalWriteLock
                .WaitAsync(options.Timeout, options.CancellationToken)
                .ConfigureAwait(options.ContinueOnCapturedContext);

            if (!ok)
                return new _FailedLockAcquisition();

            _writeOwnerThreadId = System.Environment.CurrentManagedThreadId;

            var dispose_when_done = new CompositeDisposable();

            try
            {
                // then its children
                if (CompositeLock != null)
                {
                    var children_acquisition =
                        await
                        CompositeLock.LockChildrenAsync(LockType.Write, options).ConfigureAwait(options.ContinueOnCapturedContext);

                    if (!children_acquisition.IsLockHeld)
                    {
                        children_acquisition.Dispose();

                        // couldn't acquire children, release parent lock
                        InternalWriteLock.Release();

                        return new _FailedLockAcquisition();
                    }

                    dispose_when_done.Add(children_acquisition);
                }
            }
            catch
            {
                // some error occured, release parent lock
                InternalWriteLock.Release();

                throw;
            }

            return
                new _WriteLockAcquisition(owner: this, disposeWhenDone: dispose_when_done);

        }

        #endregion
    }
}
