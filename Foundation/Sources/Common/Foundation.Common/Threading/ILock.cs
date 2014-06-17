using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public interface ILock
    {
        IReadLockAcquisition AcquireReadLock();
        IReadLockAcquisition TryAcquireReadLock(TimeSpan timeout);

        IReadLockAcquisition AcquireUpgradeableReadLock();
        IReadLockAcquisition TryAcquireUpgradeableReadLock(TimeSpan timeout);

        IWriteLockAcquisition AcquireWriteLock();
        IWriteLockAcquisition TryAcquireWriteLock(TimeSpan timeout);
    }
}
