using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    /*
     * var l = await Lock.AcquireWriteLockAsync(ct);
     * if(!l.IsLockHeld)
     *  // lock not acquired
     * else
     * {
     *  // lock acquired
     *  l.Dispose();
     * }
     * 
     * OR
     * 
     * using(var l = await Lock.AcquireWriteLockAsync(ct))
     * {
     *  if(!l.IsLockHeld)
     *  {
     *      // lock not acquired
     *  }
     *  else
     *  {
     *      // lock acquired
     *  }
     * }
     * 
     */

    public interface IAsyncLock
    {
        /// <summary>
        /// Acquires Write Lock.
        /// </summary>
        /// <returns></returns>
        Task<ILockAcquisition> AcqureWriteLockAsync(bool continueOnCapturedContext = false);
        Task<ILockAcquisition> AcqureWriteLockAsync(CancellationToken ct, bool continueOnCapturedContext = false);
        Task<ILockAcquisition> AcqureWriteLockAsync(int millisecondsTimeout, bool continueOnCapturedContext = false);
        Task<ILockAcquisition> AcqureWriteLockAsync(int millisecondsTimeout, CancellationToken ct, bool continueOnCapturedContext = false);
        Task<ILockAcquisition> AcqureWriteLockAsync(TimeSpan timeout, bool continueOnCapturedContext = false);
        Task<ILockAcquisition> AcqureWriteLockAsync(TimeSpan timeout, CancellationToken ct, bool continueOnCapturedContext = false);
    }
}
