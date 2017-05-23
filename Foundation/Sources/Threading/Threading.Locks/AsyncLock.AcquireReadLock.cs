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
        public ILockAcquisition AcquireReadLock() 
            => AcquireReadLock(SyncOptions.Default);
        public ILockAcquisition AcquireReadLock(SyncOptions options) 
            => AcquireLock(LockType.Read, options);

        public async Task<ILockAcquisition> AcquireReadLockAsync() 
            => await AcquireReadLockAsync(AsyncOptions.Default).ConfigureAwait(AsyncOptions.Default.ContinueOnCapturedContext);
        public async Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options) 
            => await AcquireLockAsync(LockType.Read, options);

        #region Acquire Read Lock (Static)

        public static async Task<ILockAcquisition> AcquireReadLockAsync(params IAsyncLock[] locks)
            => await AcquireReadLockAsync(AsyncOptions.Default, locks);

        public static async Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options, params IAsyncLock[] locks)
        {
            if (locks == null || locks.Length == 0)
                return _FailedLockAcquisition.Instance;

            var all_acquisitions =
                await
                Task.WhenAll<ILockAcquisition>(locks.Select(x => x.AcquireReadLockAsync(options)))
                .ConfigureAwait(options.ContinueOnCapturedContext);

            return new _CompositeLockAcqusition(all_acquisitions);
        }

        public static ILockAcquisition AcquireReadLock(params ILock[] locks)
            => AcquireReadLock(SyncOptions.Default, locks);

        public static ILockAcquisition AcquireReadLock(SyncOptions options, params ILock[] locks)
        {
            if (locks == null || locks.Length == 0)
                return _FailedLockAcquisition.Instance;

            var all_acquisitions = locks.Select(x => x.AcquireReadLock(options));

            return new _CompositeLockAcqusition(all_acquisitions);
        }

        #endregion
    }
}
