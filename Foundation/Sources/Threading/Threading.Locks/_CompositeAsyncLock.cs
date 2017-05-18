using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using SquaredInfinity.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SquaredInfinity.Threading.Locks
{
    class _CompositeAsyncLock : ICompositeAsyncLock
    {
        #region Child Locks

        readonly IAsyncLock ChildrenLock = new AsyncLock(supportsComposition: false);

        readonly HashSet<IAsyncLock> ChildLocks = new HashSet<IAsyncLock>();

        public void AddChild(IAsyncLock childLock)
        {
            using(ChildrenLock.AcqureWriteLockAsync(continueOnCapturedContext: true).Result)
            {
                ChildLocks.AddIfNotNull(childLock);
            }
        }

        public void RemoveChild(IAsyncLock childLock)
        {
            using (ChildrenLock.AcqureWriteLockAsync(continueOnCapturedContext: true).Result)
            {
                ChildLocks.Remove(childLock);
            }
        }

        #endregion

        #region Lock Children

        public async Task<ILockAcquisition> LockChildrenAsync(
            LockType lockType,
            int millisecondsTimeout,
            CancellationToken ct,
            bool continueOnCapturedContext = false)
        {
            var all_lock_tasks = new List<Task<ILockAcquisition>>();

            if (ChildLocks.Count == 0)
                return new _CompositeLockAcqusition();

            using (var _lock = await ChildrenLock.AcqureWriteLockAsync(millisecondsTimeout, ct, continueOnCapturedContext))
            {
                if (!_lock.IsLockHeld)
                {
                    return await Task.FromResult(new _FailedLockAcquisition());
                }

                foreach (var child in ChildLocks)
                {
                    if (lockType == LockType.Read && !(child is IReadLock))
                        lockType = LockType.Write;
                    if (lockType == LockType.UpgradeableRead && !(child is IUpgradeableReadLock))
                        lockType = LockType.Write;

                    //if (lockType == LockType.Read)
                    //{
                    //    result.AddIfNotNull((child as IReadLock).AcquireReadLockIfNotHeld());
                    //}
                    //else if (lockType == LockType.UpgradeableRead)
                    //{
                    //    result.AddIfNotNull((child as IUpgradeableReadLock).AcquireUpgradeableReadLock());
                    //}
                    //else
                    if (lockType == LockType.Write)
                    {
                        var l = child.AcqureWriteLockAsync(millisecondsTimeout, ct, continueOnCapturedContext);
                        all_lock_tasks.Add(l);
                    }
                    else
                    {
                        throw new NotSupportedException(lockType.ToString());
                    }
                }

                await Task.WhenAll(all_lock_tasks);

                // check all child locks for success
                // if any failed, it was either cancelled or timed out
                // if that's the case then release all successful locks and return failure

                var all_locks_acquisition = new _CompositeLockAcqusition(all_lock_tasks.Select(x => x.Result));

                if (all_locks_acquisition.IsLockHeld)
                    return all_locks_acquisition;
                else
                {
                    // free any already held child locks
                    all_locks_acquisition.Dispose();

                    return await Task.FromResult(new _FailedLockAcquisition());
                }
            }
        }

        #endregion 
    }
}
