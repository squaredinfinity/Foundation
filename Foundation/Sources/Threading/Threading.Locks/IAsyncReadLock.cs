using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public interface IAsyncReadLock : IReadLock
    {
        /// <summary>
        /// Acquires Write Lock asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<ILockAcquisition> AcquireReadLockAsync();
        Task<ILockAcquisition> AcquireReadLockAsync(CancellationToken ct);
        Task<ILockAcquisition> AcquireReadLockAsync(TimeSpan timeout);
        Task<ILockAcquisition> AcquireReadLockAsync(TimeSpan timeout, CancellationToken ct);
        Task<ILockAcquisition> AcquireReadLockAsync(int millisecondsTimeout);
        Task<ILockAcquisition> AcquireReadLockAsync(int millisecondsTimeout, CancellationToken ct);
        Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options);
    }
}
