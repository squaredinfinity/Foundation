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
        #region Acquire Read Lock Async

        public async Task<ILockAcquisition> AcquireReadLockAsync()
        {
            var ao = AsyncOptions.Default;

            return
                await
                AcquireReadLockAsync(ao)
                .ConfigureAwait(ao.ContinueOnCapturedContext);
        }

        public async Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options)
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

            // lock parent first
            var ok =
                await
                InternalWriteLock
                .WaitAsync(options.MillisecondsTimeout, options.CancellationToken)
                .ConfigureAwait(options.ContinueOnCapturedContext);

            if (!ok)
                return new _FailedLockAcquisition();

            var dispose_when_done = new CompositeDisposable();

            _writeOwnerThreadId = System.Environment.CurrentManagedThreadId;
            
            try
            {
                // then its children
                if (CompositeLock != null)
                {
                    var children_acquisition =
                        await
                        CompositeLock.LockChildrenAsync(LockType.Read, options)
                        .ConfigureAwait(options.ContinueOnCapturedContext);

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
