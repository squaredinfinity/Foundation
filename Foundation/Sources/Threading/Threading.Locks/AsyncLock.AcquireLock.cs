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
        public ILockAcquisition AcquireLock(LockType lockType)
            => AcquireLock(lockType, SyncOptions.Default);

        /// <summary>
        /// A common pattern of acquiring both Read and Write locks.
        /// Async Lock does not recognise differences between Write and Read locks, but its children may support both locks differently.
        /// </summary>
        /// <param name="lockType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ILockAcquisition AcquireLock(LockType lockType, SyncOptions options)
        {
            if (options.MillisecondsTimeout == 0)
                return _FailedLockAcquisition.Instance;

            if (options.CorrelationToken == null)
                options = options.CorrelateThisThread();

            // lock parent first
            var ok =
                InternalWriteLock
                .Wait(options.MillisecondsTimeout, options.CancellationToken);

            if (!ok)
            {
                return new _FailedLockAcquisition();
            }

            _AddWriter(options.CorrelationToken);

            if (IsLockAcquisitionRecursive(options.CorrelationToken))
            {
                // this lock is already held, nothing to do here
                return new _WriteLockAcquisition(this); 
                // disposables should be added to lock owner
            }

            var dispose_when_done = new CompositeDisposable();

            try
            {
                // then its children
                if (CompositeLock != null)
                {
                    var children_acquisition = CompositeLock.LockChildren(lockType, options);

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
    }
}
