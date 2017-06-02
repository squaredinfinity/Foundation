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

        /// <summary>
        /// A common patter of acquiringboth Read and Write async locks. 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<ILockAcquisition> AcquireLockAsync(LockType lockType, AsyncOptions options)
        {
            if (options.MillisecondsTimeout == 0)
                return _FailedLockAcquisition.Instance;

            if(RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                throw new NotSupportedException("Async acquisitions are not supported by recursive locks");

            // lock parent first
            var ok =
                await
                InternalWriteLock
                .WaitAsync(options.MillisecondsTimeout, options.CancellationToken)
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
                        CompositeLock.LockChildrenAsync(lockType, options)
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

            return new _WriteLockAcquisition(owner: this, disposeWhenDone: dispose_when_done);
        }
    }
}
