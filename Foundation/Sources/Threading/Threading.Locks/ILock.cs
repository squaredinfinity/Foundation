using SquaredInfinity.Disposables;
using SquaredInfinity.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{  
    public interface ILock : IWriteLock, IReadLock, ICompositeLock
    {
        long LockId { get; }
        string Name { get; }

        IReadOnlyList<LockOwnership> AllOwners { get; }

        /// <summary>
        /// Specifies recursion policy of this lock.
        /// </summary>
        LockRecursionPolicy RecursionPolicy { get; }

        LockState State { get; }

        ILockAcquisition AcquireLock(LockType lockType);
        ILockAcquisition AcquireLock(LockType lockType, SyncOptions syncOptions);
    }
}
