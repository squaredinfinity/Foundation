using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public interface IReadLockAcquisition : IDisposable
    {
        IWriteLockAcquisition AcquireWriteLock();
        bool TryAcquireWriteLock(TimeSpan timeout, out IWriteLockAcquisition writeLockAcquisition);
    }
}
