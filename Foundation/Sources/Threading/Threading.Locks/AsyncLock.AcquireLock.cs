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
            if (options.CorrelationToken == null)
                options = options.CorrelateThisThread();

            if (options.MillisecondsTimeout == 0)
                return new _FailedLockAcquisition(options.CorrelationToken);

            using (InternalLock.AcquireWriteLock())
            {
                if (_IsLockAcquisitionAttemptRecursive__NOLOCK(options.CorrelationToken))
                {
                    _AddRecursiveWriter__NOLOCK(options.CorrelationToken);
                    return new _LockAcquisition(this, options.CorrelationToken);
                }
            }

            // lock parent first
            var ok =
                InternalSemaphore
                .Wait(options.MillisecondsTimeout, options.CancellationToken);

            if (!ok)
            {
                return new _FailedLockAcquisition(options.CorrelationToken);
            }

            var dispose_when_done = new CompositeDisposable();

            try
            {
                using (InternalLock.AcquireWriteLock())
                {
                    if (_IsLockAcquisitionAttemptRecursive__NOLOCK(options.CorrelationToken))
                    {
                        _AddRecursiveWriter__NOLOCK(options.CorrelationToken);
                        return new _LockAcquisition(this, options.CorrelationToken);
                    }
                }

                // then its children
                if (CompositeLock != null)
                {
                    var children_acquisition = CompositeLock.LockChildren(lockType, options);

                    if (!children_acquisition.IsLockHeld)
                    {
                        children_acquisition.Dispose();

                        // couldn't acquire children, release parent lock
                        InternalSemaphore.Release();

                        return new _FailedLockAcquisition(options.CorrelationToken);
                    }

                    dispose_when_done.Add(children_acquisition);
                }
            }
            catch
            {
                // some error occured, release parent lock
                InternalSemaphore.Release();

                throw;
            }

            using (InternalLock.AcquireWriteLock())
            {
                _AddWriter__NOLOCK(options.CorrelationToken, dispose_when_done);
            }

            return new _LockAcquisition(this, options.CorrelationToken);
        }
    }
}
