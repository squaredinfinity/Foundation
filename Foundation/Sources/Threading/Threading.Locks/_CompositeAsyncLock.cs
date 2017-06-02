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
        public int ChildrenCount
        {
            get
            {
                using (ChildrenLock.AcquireWriteLock())
                {
                    return ChildLocks.Count;
                }
            }
        }

        #region Child Locks

        // there's no need for internal lock to support recursion (implementation of this type will avoid this need)
        // there's no need for internal lock to support composition (no child locks will be added)
        readonly IAsyncLock ChildrenLock = new AsyncLock(recursionPolicy: LockRecursionPolicy.NoRecursion, supportsComposition: false);

        readonly HashSet<ILock> ChildLocks = new HashSet<ILock>();

        public void AddChild(IAsyncLock childLock)
        {
            if (childLock == null)
                throw new ArgumentNullException(nameof(childLock));

            using(ChildrenLock.AcquireWriteLock())
            {
                ChildLocks.Add(childLock);
            }
        }

        public void AddChild(ILock childLock)
        {
            if (childLock == null)
                throw new ArgumentNullException(nameof(childLock));

            using (ChildrenLock.AcquireWriteLock())
            {
                ChildLocks.Add(childLock);
            }
        }

        public void RemoveChild(IAsyncLock childLock)
        {
            if (childLock == null)
                throw new ArgumentNullException(nameof(childLock));

            using (ChildrenLock.AcquireWriteLock())
            {
                ChildLocks.Remove(childLock);
            }
        }

        public void RemoveChild(ILock childLock)
        {
            if (childLock == null)
                throw new ArgumentNullException(nameof(childLock));

            using (ChildrenLock.AcquireWriteLock())
            {
                ChildLocks.Remove(childLock);
            }
        }

        #endregion

        #region Lock Children Async

        public async Task<ILockAcquisition> LockChildrenAsync(
            LockType lockType,
            AsyncOptions options)
        {
            var all_lock_tasks = new List<Task<ILockAcquisition>>();

            if (ChildLocks.Count == 0)
                return new _CompositeLockAcqusition();

            using (var _lock = 
                await 
                ChildrenLock.AcquireWriteLockAsync(options)
                .ConfigureAwait(options.ContinueOnCapturedContext))
            {
                if (!_lock.IsLockHeld)
                {
                    return await Task.FromResult(new _FailedLockAcquisition());
                }

                foreach (var child in ChildLocks)
                {
                    if (lockType == LockType.Read)
                    {
                        var l = (Task<ILockAcquisition>)null;

                        switch (child)
                        {
                            case IAsyncLock async_child:
                                l = async_child.AcquireReadLockAsync(options);
                                break;
                            case ILock sync_child:
                                l = new Task<ILockAcquisition>(() => sync_child.AcquireReadLock(new SyncOptions(options)));
                                break;
                            default:
                                throw new NotSupportedException($"specified lock type is not supported: {child.GetType().FullName}");
                        }

                        all_lock_tasks.Add(l);
                    }
                    else if (lockType == LockType.Write)
                    {
                        var l = (Task<ILockAcquisition>)null;

                        switch(child)
                        {
                            case IAsyncLock async_child:
                                l = async_child.AcquireWriteLockAsync(options);
                                break;
                            case ILock sync_child:
                                l = new Task<ILockAcquisition>(() => sync_child.AcquireWriteLock(new SyncOptions(options)));
                                break;
                            default:
                                throw new NotSupportedException($"specified lock type is not supported: {child.GetType().FullName}");
                        }

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

        #region Lock Children

        public ILockAcquisition LockChildren(
            LockType lockType,
            SyncOptions options)
        {
            if (ChildLocks.Count == 0)
            {
                return new _CompositeLockAcqusition();
            }

            using (var _lock = ChildrenLock.AcquireWriteLock(options))
            {
                if (!_lock.IsLockHeld)
                {
                    return new _FailedLockAcquisition();
                }

                var all_child_acquisitions = new List<ILockAcquisition>(capacity: ChildLocks.Count);

                foreach (var child in ChildLocks)
                {
                    if (lockType == LockType.Read)
                    {
                        var l = child.AcquireReadLock(options);

                        // do not attempt to acquire other locks if this lock failed

                        if (l.IsLockHeld == false)
                            break;

                        all_child_acquisitions.Add(l);
                    }
                    else if (lockType == LockType.Write)
                    {
                        var l = child.AcquireWriteLock(options);

                        // do not attempt to acquire other locks if this lock failed

                        if (l.IsLockHeld == false)
                            break;

                        all_child_acquisitions.Add(l);
                    }
                    else
                    {
                        throw new NotSupportedException(lockType.ToString());
                    }
                }

                // check all child locks for success
                // if any failed, it was either cancelled or timed out
                // if that's the case then release all successful locks and return failure

                var all_locks_acquisition = new _CompositeLockAcqusition(all_child_acquisitions);

                if (all_locks_acquisition.IsLockHeld)
                {
                    return all_locks_acquisition;
                }
                else
                {
                    // free any already held child locks
                    all_locks_acquisition.Dispose();

                    return new _FailedLockAcquisition();
                }
            }
        }

        #endregion
    }
}
