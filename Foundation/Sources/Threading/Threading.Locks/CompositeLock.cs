using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using SquaredInfinity.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public abstract class CompositeLock : ICompositeLock
    {
        #region Child Locks

        readonly object ChildLocksSync = new object();
        readonly HashSet<ILock> ChildLocks = new HashSet<ILock>();

        public void AddChild(ILock childLock)
        {
            lock (ChildLocksSync)
            {
                ChildLocks.AddIfNotNull(childLock);
            }
        }

        public void RemoveChild(ILock childLock)
        {
            lock (ChildLocksSync)
            {
                ChildLocks.Remove(childLock);
            }
        }

        #endregion

        #region (un)lock children

        protected virtual IDisposable LockChildren(LockModes lockType)
        {
            var result = new CompositeDisposable();

            lock (ChildLocksSync)
            {
                foreach (var child in ChildLocks)
                {
                    if (lockType == LockModes.Read && !(child is IReadLock))
                        lockType = LockModes.Write;
                    if (lockType == LockModes.UpgradeableRead && !(child is IUpgradeableReadLock))
                        lockType = LockModes.Write;

                    if (lockType == LockModes.Read)
                    {
                        result.AddIfNotNull((child as IReadLock).AcquireReadLockIfNotHeld());
                    }
                    else if (lockType == LockModes.UpgradeableRead)
                    {
                        result.AddIfNotNull((child as IUpgradeableReadLock).AcquireUpgradeableReadLock());
                    }
                    else if (lockType == LockModes.Write)
                    {
                        result.AddIfNotNull(child.AcquireWriteLockIfNotHeld());
                    }
                    else
                    {
                        throw new NotSupportedException(lockType.ToString());
                    }
                }
            }

            return result;
        }


        public virtual bool TryLockChildren(LockModes lockType, TimeSpan timeout, out IDisposable childDisposables)
        {
            var all_child_disposables = new CompositeDisposable();
            childDisposables = all_child_disposables;

            bool all_good = true;

            foreach (var child in ChildLocks)
            {
                if (!all_good)
                    break;

                var actual_lock_type = lockType;

                if (lockType == LockModes.Read && !(child is IReadLock))
                    lockType = LockModes.Write;
                if (lockType == LockModes.UpgradeableRead && !(child is IUpgradeableReadLock))
                    lockType = LockModes.Write;

                if (lockType == LockModes.Read)
                {
                    var acqusition = (IReadLockAcquisition)null;
                    if ((child as IReadLock).TryAcquireReadLock(timeout, out acqusition))
                    {
                        all_child_disposables.AddIfNotNull(acqusition);
                    }
                    else
                    {
                        all_good = false;
                    }
                }
                else if (lockType == LockModes.UpgradeableRead)
                {
                    var acqusition = (IUpgradeableReadLockAcquisition)null;
                    if ((child as IUpgradeableReadLock).TryAcquireUpgradeableReadLock(timeout, out acqusition))
                    {
                        all_child_disposables.AddIfNotNull(acqusition);
                    }
                    else
                    {
                        all_good = false;
                    }
                }
                else if (lockType == LockModes.Write)
                {
                    var acqusition = (IWriteLockAcquisition)null;
                    if (child.TryAcquireWriteLock(timeout, out acqusition))
                    {
                        all_child_disposables.AddIfNotNull(acqusition);
                    }
                    else
                    {
                        all_good = false;
                    }
                }
                else
                {
                    throw new NotSupportedException(lockType.ToString());
                }
            }

            if (all_good)
            {
                return true;
            }
            else
            {
                all_child_disposables.Clear();
                return false;
            }
        }

        protected virtual void UnlockChildren(IDisposable childLocks)
        {
            childLocks.Dispose();
        }

        #endregion 
    }
}
