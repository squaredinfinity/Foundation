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
    public partial class AsyncLock : IAsyncLock, ICompositeAsyncLock
    {        
        #region Acquire Write Lock

        public ILockAcquisition AcquireWriteLock() => AcquireWriteLock(SyncOptions.Default);

        public ILockAcquisition AcquireWriteLock(SyncOptions options)
        {
            if (RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
            {
                if (WriteOwnerThreadId == System.Environment.CurrentManagedThreadId)
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
            else if(System.Environment.CurrentManagedThreadId == WriteOwnerThreadId)
            {
                // cannot acquire non-recursive lock from thread which already owns it.
                throw new LockRecursionException();
            }

            // lock parent first
            var ok =
                InternalWriteLock
                .Wait(options.MillisecondsTimeout, options.CancellationToken);

            if (!ok)
            {
                return new _FailedLockAcquisition();
            }

            _writeOwnerThreadId = System.Environment.CurrentManagedThreadId;

            var dispose_when_done = new CompositeDisposable();

            try
            {
                // then its children
                if (CompositeLock != null)
                {
                    var children_acquisition = (ILockAcquisition)null;

                    CompositeLock.TryLockChildren(LockType.Write, options, out children_acquisition);

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

            return new _WriteLockAcquisition(owner: this, disposeWhenDone: dispose_when_done);
        }

        #endregion
    }
}
