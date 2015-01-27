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

        public bool IsReadLockHeld { get { return InternalLock.IsReadLockHeld; } }
        public bool IsUpgradeableReadLockHeld { get { return InternalLock.IsUpgradeableReadLockHeld; } }
        public bool IsWriteLockHeld { get { return InternalLock.IsWriteLockHeld; } }
        
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

            return new ReadLockAcquisition(owner: this);
        }

        public bool TryAcquireReadLock(TimeSpan timeout, out IReadLockAcquisition readLockAcquisition)
        {
            if (InternalLock.RecursionPolicy == LockRecursionPolicy.NoRecursion &&
                (InternalLock.IsReadLockHeld || InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld))
            {
                readLockAcquisition = null;
                return false;
            }

            if (InternalLock.TryEnterReadLock(timeout))
            {
                readLockAcquisition = new ReadLockAcquisition(owner: this);
                return true;
            }
            else
            {
                readLockAcquisition = null;
                return false;
            }
            
        }

        public IUpgradeableReadLockAcquisition AcquireUpgradeableReadLock()
        {
            InternalLock.EnterUpgradeableReadLock();

            return new UpgradeableReadLockAcquisition(owner: this);
        }

        public bool TryAcquireUpgradeableReadLock(TimeSpan timeout, out IUpgradeableReadLockAcquisition upgradeableReadLockAcquisition)
        {
            if (InternalLock.RecursionPolicy == LockRecursionPolicy.NoRecursion &&
                (InternalLock.IsReadLockHeld || InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld))
            {
                upgradeableReadLockAcquisition = null;
                return false;
            }

            if (InternalLock.RecursionPolicy == LockRecursionPolicy.SupportsRecursion &&
                !(InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld))
            {
                upgradeableReadLockAcquisition = null;
                return false;
            }

            if(InternalLock.TryEnterUpgradeableReadLock(timeout))
            {
                upgradeableReadLockAcquisition = new UpgradeableReadLockAcquisition(owner: this);
                return true;
            }
            else
            {
                upgradeableReadLockAcquisition = null;
                return false;
            }
        }

        public IWriteLockAcquisition AcquireWriteLock()
        {
            InternalLock.EnterWriteLock();

            return new WriteLockAcquisition(owner:this);
        }

        public IWriteLockAcquisition AcquireWriteLockIfNotHeld()
        {
            if (InternalLock.IsWriteLockHeld)
                return null;

            InternalLock.EnterWriteLock();

            return new WriteLockAcquisition(owner: this);
        }

        public bool TryAcquireWriteLock(TimeSpan timeout, out IWriteLockAcquisition writeableLockAcquisition)
        {
            if (InternalLock.RecursionPolicy == LockRecursionPolicy.NoRecursion &&
                (InternalLock.IsReadLockHeld || InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld))
            {
                writeableLockAcquisition = null;
                return false;
            }

            if (InternalLock.RecursionPolicy == LockRecursionPolicy.SupportsRecursion &&
                !(InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld))
            {
                writeableLockAcquisition = null;
                return false;
            }

            if (InternalLock.TryEnterWriteLock(timeout))
            {
                writeableLockAcquisition = new WriteLockAcquisition(owner: this);
                return true;
            }
            else
            {
                writeableLockAcquisition = null;
                return false;
            }
        }
    }
}
