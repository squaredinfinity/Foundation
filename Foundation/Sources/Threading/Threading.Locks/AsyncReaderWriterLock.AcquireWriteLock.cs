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
            => AcquireReadLock(new SyncOptions(timeout, ct));

        public ILockAcquisition AcquireWriteLock(SyncOptions options) => AcquireLock(LockType.Write, options);

        #endregion

        #region Acquire Write Lock Asnyc (overloads)

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

        public async Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options) => await AcquireLockAsync(LockType.Write, options);

        #endregion
    }
}
