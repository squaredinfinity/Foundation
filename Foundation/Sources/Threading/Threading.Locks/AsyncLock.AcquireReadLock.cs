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
        #region Acquire Read Lock

        public ILockAcquisition AcquireReadLock() => AcquireReadLock(SyncOptions.Default);

        public ILockAcquisition AcquireReadLock(SyncOptions options)
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
                InternalWriteLock
                .Wait(options.MillisecondsTimeout, options.CancellationToken);

            if (!ok)
            {
                return new _FailedLockAcquisition();
            }

            var dispose_when_done = new CompositeDisposable();

            _writeOwnerThreadId = System.Environment.CurrentManagedThreadId;

            try
            {
                // then its children
                if (CompositeLock != null)
                {
                    var children_acquisition = (ILockAcquisition)null;

                    CompositeLock.TryLockChildren(LockType.Read, options, out children_acquisition);

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
