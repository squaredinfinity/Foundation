using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public interface IUpgradeableReadLockAcquisition : IDisposable
    {
        IReadLockAcquisition DowngradeToReadLock();
        bool TryDowngradeToReadLock(TimeSpan timeout, out IReadLockAcquisition readLockAcquisition);

        IWriteLockAcquisition UpgradeToWriteLock();
        bool TryUpgradeToWriteLock(TimeSpan timeout, out IWriteLockAcquisition writeLockAcquisition);
    }
}
