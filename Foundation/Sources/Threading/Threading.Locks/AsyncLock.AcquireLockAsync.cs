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
        public async Task<ILockAcquisition> AcquireLockAsync(LockType lockType)
            => await AcquireLockAsync(lockType, AsyncOptions.Default);

        public async Task<ILockAcquisition> AcquireLockAsync(LockType lockType, ICorrelationToken correlationToken)
            => await AcquireLockAsync(lockType, AsyncOptions.Default.Correlate(correlationToken ?? throw new ArgumentNullException(nameof(correlationToken))));

        /// <summary>
        /// A common patter of acquiringboth Read and Write async locks. 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<ILockAcquisition> AcquireLockAsync(LockType lockType, AsyncOptions options)
        {
            if (options.MillisecondsTimeout == 0)
                return new _FailedLockAcquisition(options.CorrelationToken);

            if (options.CorrelationToken == null)
                options = options.Correlate(new CorrelationToken("__automatic_async__"));

            using (InternalLock.AcquireWriteLock())
            {
                if (_IsLockAcquisitionAttemptRecursive__NOLOCK(options.CorrelationToken))
                {
                    if (RecursionPolicy == LockRecursionPolicy.NoRecursion)
                        throw new LockRecursionException();

                    _AddRecursiveWriter__NOLOCK(options.CorrelationToken);
                    return new _LockAcquisition(this, options.CorrelationToken);
                }
            }

            // lock parent first
            var ok =
                await
                InternalSemaphore
                .WaitAsync(options.MillisecondsTimeout, options.CancellationToken)
                .ConfigureAwait(options.ContinueOnCapturedContext);

            if (!ok)
                return new _FailedLockAcquisition(options.CorrelationToken);
            
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
                    var children_acquisition =
                        await
                        CompositeLock.LockChildrenAsync(lockType, options)
                        .ConfigureAwait(options.ContinueOnCapturedContext);

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
