using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public partial class ReaderWriterLockSlimEx : ILock
    {
        readonly internal ReaderWriterLockSlim InternalLock;

        public string Name { get; private set; }

        public bool IsReadLockHeld { get { return InternalLock.IsReadLockHeld; } }
        public bool IsUpgradeableReadLockHeld { get { return InternalLock.IsUpgradeableReadLockHeld; } }
        public bool IsWriteLockHeld { get { return InternalLock.IsWriteLockHeld; } }

        #region Child Locks

        readonly HashSet<ILock> ChildLocks = new HashSet<ILock>();

        public void AddChild(ILock childLock)
        {
            ChildLocks.AddIfNotNull(childLock);
        }

        public void RemoveChild(ILock childLock)
        {
            ChildLocks.Remove(childLock);
        }

        #endregion

        public ReaderWriterLockSlimEx(string name, LockRecursionPolicy recursionPolicy = LockRecursionPolicy.NoRecursion)
            : this(name, new ReaderWriterLockSlim(recursionPolicy))
        { }

        public ReaderWriterLockSlimEx(LockRecursionPolicy recursionPolicy = LockRecursionPolicy.NoRecursion)
            : this(new ReaderWriterLockSlim(recursionPolicy))
        { }

        public ReaderWriterLockSlimEx(ReaderWriterLockSlim readerWriterLock)
            : this(Guid.NewGuid().ToString(), readerWriterLock)
        {}

        public ReaderWriterLockSlimEx(string name, ReaderWriterLockSlim readerWriterLock)
        {
            this.Name = name;
            this.InternalLock = readerWriterLock;
        }


        protected virtual IDisposable LockChildren(LockTypes lockType)
        {
            var result = new CompositeDisposable();

            foreach(var child in ChildLocks)
            {
                if(lockType == LockTypes.Read)
                {
                    result.AddIfNotNull(child.AcquireReadLockIfNotHeld());
                }
                else if(lockType == LockTypes.UpgradeableRead)
                {
                    result.AddIfNotNull(child.AcquireUpgradeableReadLock());
                }
                else if(lockType == LockTypes.Write)
                {
                    result.AddIfNotNull(child.AcquireWriteLockIfNotHeld());
                }
                else
                {
                    throw new NotSupportedException(lockType.ToString());
                }
            }

            return result;
        }


        public virtual bool TryLockChildren(LockTypes lockType, TimeSpan timeout, out IDisposable childDisposables)
        {
            var all_child_disposables = new CompositeDisposable();
            childDisposables = all_child_disposables;

            bool all_good = true;

            foreach(var child in ChildLocks)
            {
                if (!all_good)
                    break;

                if (lockType == LockTypes.Read)
                {
                    var acqusition = (IReadLockAcquisition)null;
                    if (child.TryAcquireReadLock(timeout, out acqusition))
                    {
                        all_child_disposables.AddIfNotNull(acqusition);
                    }
                    else
                    {
                        all_good = false;
                    }
                }
                else if (lockType == LockTypes.UpgradeableRead)
                {
                    var acqusition = (IUpgradeableReadLockAcquisition)null;
                    if (child.TryAcquireUpgradeableReadLock(timeout, out acqusition))
                    {
                        all_child_disposables.AddIfNotNull(acqusition);
                    }
                    else
                    {
                        all_good = false;
                    }
                }
                else if (lockType == LockTypes.Write)
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

            if(all_good)
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

        public IReadLockAcquisition AcquireReadLock()
        {
            // lock parent first
            InternalLock.EnterReadLock();
            // then its children
            var disposables = LockChildren(LockTypes.Read);

            return new ReadLockAcquisition(owner: this, disposeWhenDone: disposables);
        }

        public IReadLockAcquisition AcquireReadLockIfNotHeld()
        {
            var isAnyLockHeld = InternalLock.IsReadLockHeld || InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld;

            if(isAnyLockHeld)
                return null;

            // lock parent first
            InternalLock.EnterReadLock();
            // then its children
            var disposables = LockChildren(LockTypes.Read);

            return new ReadLockAcquisition(owner: this, disposeWhenDone: disposables);
        }

        public bool TryAcquireReadLock(TimeSpan timeout, out IReadLockAcquisition readLockAcquisition)
        {
            if (InternalLock.RecursionPolicy == LockRecursionPolicy.NoRecursion &&
                (InternalLock.IsReadLockHeld || InternalLock.IsUpgradeableReadLockHeld || InternalLock.IsWriteLockHeld))
            {
                readLockAcquisition = null;
                return false;
            }

            var disposables = (IDisposable)null;
            var ok = TryLockChildren(LockTypes.Read, timeout, out disposables);

            if (ok)
            {
                ok = InternalLock.TryEnterReadLock(timeout);

                if (ok)
                {
                    readLockAcquisition = new ReadLockAcquisition(owner: this, disposeWhenDone: LockChildren(LockTypes.Read));
                    return true;
                }
                else
                {
                    disposables?.Dispose();
                }
            }

            readLockAcquisition = null;
            return false;
        }

        public IUpgradeableReadLockAcquisition AcquireUpgradeableReadLock()
        {
            // lock parent first
            InternalLock.EnterUpgradeableReadLock();
            // then its children
            var disposables = LockChildren(LockTypes.UpgradeableRead);

            return new UpgradeableReadLockAcquisition(owner: this, disposeWhenDone: disposables);
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

            IDisposable disposables = (IDisposable)null;

            var ok = TryLockChildren(LockTypes.UpgradeableRead, timeout, out disposables);

            if (ok)
            {
                ok = InternalLock.TryEnterUpgradeableReadLock(timeout);

                if (ok)
                {
                    upgradeableReadLockAcquisition = new UpgradeableReadLockAcquisition(owner: this, disposeWhenDone: disposables);
                    return true;
                }
                else
                {
                    disposables?.Dispose();
                }
            }

            upgradeableReadLockAcquisition = null;
            return false;
        }

        public IWriteLockAcquisition AcquireWriteLock()
        {
            // lock parent first
            InternalLock.EnterWriteLock();
            // then its children
            var disposables = LockChildren(LockTypes.Write);

            return new WriteLockAcquisition(owner: this, disposeWhenDone: disposables);
        }

        public IWriteLockAcquisition AcquireWriteLockIfNotHeld()
        {
            if (InternalLock.IsWriteLockHeld)
                return null;

            // lock parent first
            InternalLock.EnterWriteLock();
            // then its children
            var disposables = LockChildren(LockTypes.Write);

            return new WriteLockAcquisition(owner: this, disposeWhenDone: disposables);
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

            var disposables = (IDisposable)null;

            var ok = TryLockChildren(LockTypes.Write, timeout, out disposables);

            if (ok)
            {
                ok = InternalLock.TryEnterWriteLock(timeout);

                if (ok)
                {
                    writeableLockAcquisition = new WriteLockAcquisition(owner: this, disposeWhenDone: disposables);
                    return true;
                }
                else
                {
                    disposables?.Dispose();
                }
            }

            writeableLockAcquisition = null;
            return false;
        }

        public string DebuggerDisplay
        {
            get { return $"{Name}, r: {IsReadLockHeld}, ur: {IsUpgradeableReadLockHeld}, w: {IsWriteLockHeld}"; }
        }
    }
}
