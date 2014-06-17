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

            return new ReadLockAcquisition(owner: this, isSuccesfull: true);
        }

        public IReadLockAcquisition TryAcquireReadLock(TimeSpan timeout)
        {
            if (InternalLock.RecursionPolicy == LockRecursionPolicy.NoRecursion && 
                (InternalLock.IsReadLockHeld || InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld))
            {
                return new ReadLockAcquisition(owner: this, isSuccesfull: false);
            }

            bool ok = InternalLock.TryEnterReadLock(timeout);
            
            return new ReadLockAcquisition(owner: this, isSuccesfull: ok);
            
        }

        public IReadLockAcquisition AcquireUpgradeableReadLock()
        {
            InternalLock.EnterUpgradeableReadLock();

            return new UpgradeableReadLockAcquisition(owner: this, isSuccesfull: true);
        }

        public IReadLockAcquisition TryAcquireUpgradeableReadLock(TimeSpan timeout)
        {
            if (InternalLock.RecursionPolicy == LockRecursionPolicy.NoRecursion && 
                (InternalLock.IsReadLockHeld || InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld))
            {
                return new ReadLockAcquisition(owner: this, isSuccesfull: false);
            }

            if (InternalLock.RecursionPolicy == LockRecursionPolicy.SupportsRecursion &&
                !(InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld))
            {
                return new ReadLockAcquisition(owner: this, isSuccesfull: false);
            }

            var ok = InternalLock.TryEnterUpgradeableReadLock(timeout);

            return new UpgradeableReadLockAcquisition(owner: this, isSuccesfull: ok);
        }

        public IWriteLockAcquisition AcquireWriteLock()
        {
            InternalLock.EnterWriteLock();

            return new WriteLockAcquisition(owner:this, isSuccesfull: true);
        }

        public IWriteLockAcquisition TryAcquireWriteLock(TimeSpan timeout)
        {
            if (InternalLock.RecursionPolicy == LockRecursionPolicy.NoRecursion && 
                (InternalLock.IsReadLockHeld || InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld))
            {
                return new WriteLockAcquisition(owner: this, isSuccesfull: false);
            }

            if (InternalLock.RecursionPolicy == LockRecursionPolicy.SupportsRecursion &&
                !(InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld))
            {
                return new WriteLockAcquisition(owner: this, isSuccesfull: false);
            }

            var ok = InternalLock.TryEnterWriteLock(timeout);

            return new WriteLockAcquisition(owner: this, isSuccesfull: ok);
        }
    }
}
