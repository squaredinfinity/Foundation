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
        #region Acquire Write Lock (overloads)

        public ILockAcquisition AcquireWriteLock()
            => AcquireWriteLock(SyncOptions.Default);
        public ILockAcquisition AcquireWriteLock(CancellationToken ct)
            => AcquireWriteLock(new SyncOptions(ct));
        public ILockAcquisition AcquireWriteLock(int millisecondsTimeout)
            => AcquireWriteLock(new SyncOptions(millisecondsTimeout));
        public ILockAcquisition AcquireWriteLock(int millisecondsTimeout, CancellationToken ct)
            => AcquireWriteLock(new SyncOptions(millisecondsTimeout, ct));
        public ILockAcquisition AcquireWriteLock(TimeSpan timeout)
            => AcquireWriteLock(new SyncOptions(timeout));
        public ILockAcquisition AcquireWriteLock(TimeSpan timeout, CancellationToken ct)
            => AcquireWriteLock(new SyncOptions(timeout, ct));
        public ILockAcquisition AcquireWriteLock(SyncOptions options)
            => AcquireLock(LockType.Write, options);

        #endregion

        #region Acquire Write Lock Async (overloads)

        public async Task<ILockAcquisition> AcquireWriteLockAsync()
            => await AcquireWriteLockAsync(AsyncOptions.Default);
        public async Task<ILockAcquisition> AcquireWriteLockAsync(CancellationToken ct)
            => await AcquireWriteLockAsync(new AsyncOptions(ct));
        public async Task<ILockAcquisition> AcquireWriteLockAsync(int millisecondsTimeout)
            => await AcquireWriteLockAsync(new AsyncOptions(millisecondsTimeout));
        public async Task<ILockAcquisition> AcquireWriteLockAsync(int millisecondsTimeout, CancellationToken ct)
            => await AcquireWriteLockAsync(new AsyncOptions(millisecondsTimeout, ct));
        public async Task<ILockAcquisition> AcquireWriteLockAsync(TimeSpan timeout)
            => await AcquireWriteLockAsync(new AsyncOptions(timeout));
        public async Task<ILockAcquisition> AcquireWriteLockAsync(TimeSpan timeout, CancellationToken ct)
            => await AcquireWriteLockAsync(new AsyncOptions(timeout, ct));
        public async Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options)
            => await AcquireLockAsync(LockType.Write, options);

        #endregion
        
        #region Acquire Write Lock (Static)

        public static async Task<ILockAcquisition> AcquireWriteLockAsync(params IAsyncLock[] locks)
            => await AcquireWriteLockAsync(AsyncOptions.Default, locks);

        public static async Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options, params IAsyncLock[] locks)
        {
            if (locks == null || locks.Length == 0)
                return new _FailedLockAcquisition(options.CorrelationToken);

            var all_acquisitions =
                await
                Task.WhenAll<ILockAcquisition>(locks.Select(x => x.AcquireWriteLockAsync(options)))
                .ConfigureAwait(options.ContinueOnCapturedContext);

            return new _CompositeLockAcqusition(options.CorrelationToken, all_acquisitions);
        }

        public static ILockAcquisition AcquireWriteLock(params ILock[] locks)
            => AcquireWriteLock(SyncOptions.Default, locks);

        public static ILockAcquisition AcquireWriteLock(SyncOptions options, params ILock[] locks)
        {
            if (locks == null || locks.Length == 0)
                return new _FailedLockAcquisition(options.CorrelationToken);

            var all_acquisitions = locks.Select(x => x.AcquireReadLock(options));

            return new _CompositeLockAcqusition(options.CorrelationToken, all_acquisitions);
        }

        #endregion
    }
}
