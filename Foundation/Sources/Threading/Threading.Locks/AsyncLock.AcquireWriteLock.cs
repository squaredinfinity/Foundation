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
        public ILockAcquisition AcquireWriteLock() 
            => AcquireWriteLock(SyncOptions.Default);
        public ILockAcquisition AcquireWriteLock(SyncOptions options) 
            => AcquireLock(LockType.Write, options);

        public async Task<ILockAcquisition> AcquireWriteLockAsync()
            => await AcquireWriteLockAsync(AsyncOptions.Default).ConfigureAwait(AsyncOptions.Default.ContinueOnCapturedContext);
        public async Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options)
            => await AcquireLockAsync(LockType.Write, options);

        #region Acquire Write Lock (Static)

        public static async Task<ILockAcquisition> AcquireWriteLockAsync(params IAsyncLock[] locks)
            => await AcquireWriteLockAsync(AsyncOptions.Default, locks);

        public static async Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options, params IAsyncLock[] locks)
        {
            if (locks == null || locks.Length == 0)
                return _FailedLockAcquisition.Instance;

            var all_acquisitions =
                await
                Task.WhenAll<ILockAcquisition>(locks.Select(x => x.AcquireWriteLockAsync(options)))
                .ConfigureAwait(options.ContinueOnCapturedContext);

            return new _CompositeLockAcqusition(all_acquisitions);
        }

        public static ILockAcquisition AcquireWriteLock(params ILock[] locks)
            => AcquireWriteLock(SyncOptions.Default, locks);

        public static ILockAcquisition AcquireWriteLock(SyncOptions options, params ILock[] locks)
        {
            if (locks == null || locks.Length == 0)
                return _FailedLockAcquisition.Instance;

            var all_acquisitions = locks.Select(x => x.AcquireReadLock(options));

            return new _CompositeLockAcqusition(all_acquisitions);
        }

        #endregion
    }
}
