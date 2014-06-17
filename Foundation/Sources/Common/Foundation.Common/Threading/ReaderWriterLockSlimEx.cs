using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public partial class ReaderWriterLockSlimEx : ILock
    {
        readonly internal ReaderWriterLockSlim InternalLock;
        
        public ReaderWriterLockSlimEx(LockRecursionPolicy recursionPolicy = LockRecursionPolicy.NoRecursion)
            : this(new ReaderWriterLockSlim(recursionPolicy))
        { }

        public ReaderWriterLockSlimEx(ReaderWriterLockSlim readerWriterLock)
        {
            this.InternalLock = readerWriterLock;
        }

        public IReadLockAcquisition AcquireReadLock()
        {
            InternalLock.EnterReadLock();

            return new ReadLockAcquisition(this);
        }

        public IReadLockAcquisition AcquireUpgradeableReadLock()
        {
            InternalLock.EnterUpgradeableReadLock();

            return new UpgradeableReadLockAcquisition(this);
        }

        public IWriteLockAcquisition AcquireWriteLock()
        {
            InternalLock.EnterWriteLock();

            return new WriteLockAcquisition(this);
        }
    }
}
