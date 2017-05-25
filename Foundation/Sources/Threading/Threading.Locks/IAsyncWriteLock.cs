using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public interface IAsyncWriteLock
    {
        /// <summary>
        /// Acquires Write Lock asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<ILockAcquisition> AcquireWriteLockAsync();
        Task<ILockAcquisition> AcquireWriteLockAsync(CancellationToken ct);
        Task<ILockAcquisition> AcquireWriteLockAsync(TimeSpan timeout);
        Task<ILockAcquisition> AcquireWriteLockAsync(TimeSpan timeout, CancellationToken ct);
        Task<ILockAcquisition> AcquireWriteLockAsync(int millisecondsTimeout);
        Task<ILockAcquisition> AcquireWriteLockAsync(int millisecondsTimeout, CancellationToken ct);
        Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options);
    }
}
