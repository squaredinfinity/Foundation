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
        #region Acquire Read Lock (overloads)

        public ILockAcquisition AcquireReadLock() 
            => AcquireReadLock(SyncOptions.Default);
        public ILockAcquisition AcquireReadLock(CancellationToken ct)
            => AcquireReadLock(new SyncOptions(ct));
        public ILockAcquisition AcquireReadLock(int millisecondsTimeout)
            => AcquireReadLock(new SyncOptions(millisecondsTimeout));
        public ILockAcquisition AcquireReadLock(int millisecondsTimeout, CancellationToken ct)
            => AcquireReadLock(new SyncOptions(millisecondsTimeout, ct));
        public ILockAcquisition AcquireReadLock(TimeSpan timeout)
            => AcquireReadLock(new SyncOptions(timeout));
        public ILockAcquisition AcquireReadLock(TimeSpan timeout, CancellationToken ct)
            => AcquireReadLock(new SyncOptions(timeout, ct));
        public ILockAcquisition AcquireReadLock(SyncOptions options) 
            => AcquireLock(LockType.Read, options);

        #endregion

        #region Acquire Read Lock Async (overloads)

        public async Task<ILockAcquisition> AcquireReadLockAsync() 
            => await AcquireReadLockAsync(AsyncOptions.Default);
        public async Task<ILockAcquisition> AcquireReadLockAsync(CancellationToken ct)
            => await AcquireReadLockAsync(new AsyncOptions(ct));
        public async Task<ILockAcquisition> AcquireReadLockAsync(int millisecondsTimeout)
            => await AcquireReadLockAsync(new AsyncOptions(millisecondsTimeout));
        public async Task<ILockAcquisition> AcquireReadLockAsync(int millisecondsTimeout, CancellationToken ct)
            => await AcquireReadLockAsync(new AsyncOptions(millisecondsTimeout, ct));
        public async Task<ILockAcquisition> AcquireReadLockAsync(TimeSpan timeout)
            => await AcquireReadLockAsync(new AsyncOptions(timeout));
        public async Task<ILockAcquisition> AcquireReadLockAsync(TimeSpan timeout, CancellationToken ct)
            => await AcquireReadLockAsync(new AsyncOptions(timeout, ct));
        public async Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options) 
            => await AcquireLockAsync(LockType.Read, options);

        #endregion

        #region Acquire Read Lock (Static)

        /// <summary>
        /// Asynchronously acquires multiple locks.
        /// Does not guarantee that each lock will be acquired on different thread (e.g. when multiple read lock are requested for ReaderWriterLock)
        /// </summary>
        /// <param name="locks"></param>
        /// <returns></returns>
        public static async Task<ILockAcquisition> AcquireReadLockAsync(params IAsyncLock[] locks)
            => await AcquireReadLockAsync(AsyncOptions.Default, locks);

        public static async Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options, params IAsyncLock[] locks)
        {
            if (locks == null || locks.Length == 0)
                return new _FailedLockAcquisition(options.CorrelationToken);

            var all_acquisitions =
                await
                Task.WhenAll<ILockAcquisition>(locks.Select(x => x.AcquireReadLockAsync(options)))
                .ConfigureAwait(options.ContinueOnCapturedContext);

            return new _CompositeLockAcqusition(options.CorrelationToken, all_acquisitions);
        }

        public static ILockAcquisition AcquireReadLock(params ILock[] locks)
            => AcquireReadLock(SyncOptions.Default, locks);

        public static ILockAcquisition AcquireReadLock(SyncOptions options, params ILock[] locks)
        {
            if (locks == null || locks.Length == 0)
                return new _FailedLockAcquisition(options.CorrelationToken);

            var all_acquisitions = locks.Select(x => x.AcquireReadLock(options));

            return new _CompositeLockAcqusition(options.CorrelationToken, all_acquisitions);
        }

        #endregion
    }
}
