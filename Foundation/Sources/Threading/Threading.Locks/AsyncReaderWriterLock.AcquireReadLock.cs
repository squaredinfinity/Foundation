using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public partial class AsyncReaderWriterLock
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

        public ILockAcquisition AcquireReadLock(SyncOptions options) => AcquireLock(LockType.Read, options);

        #endregion

        #region Acquire Read Lock (overloads)

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

        public async Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options) => await AcquireLockAsync(LockType.Read, options);

        #endregion
    }
}
