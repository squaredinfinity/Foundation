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
        bool TryAcquireReadLock(TimeSpan timeout, out IReadLockAcquisition readLockAcquisition);

        IUpgradeableReadLockAcquisition AcquireUpgradeableReadLock();
        bool TryAcquireUpgradeableReadLock(TimeSpan timeout, out IUpgradeableReadLockAcquisition upgradeableReadLockAcquisition);

        IWriteLockAcquisition AcquireWriteLock();
        bool TryAcquireWriteLock(TimeSpan timeout, out IWriteLockAcquisition writeLockAcquisition);

        bool IsReadLockHeld { get; }
        bool IsUpgradeableReadLockHeld { get; }
        bool IsWriteLockHeld { get; }
    }
}
