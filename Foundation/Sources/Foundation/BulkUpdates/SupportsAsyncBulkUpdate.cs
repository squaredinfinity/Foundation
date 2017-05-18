using SquaredInfinity.ComponentModel;
using SquaredInfinity.Disposables;
using SquaredInfinity.Threading.Locks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public class SupportsAsyncBulkUpdate : NotifyPropertyChangedObject, ISupportsAsyncBulkUpdate
    {
        readonly IAsyncLock _lock;
        public IAsyncLock Lock => _lock;

        /// <summary>
        /// Counts the number of bulk updates in progress.
        /// When a lock is specified, normally only one update can happen at the time.
        /// When no lock is specified, several updates can happen (but they risk overriding each other changes).
        /// </summary>
        int BulkUpdateCount = 0;

        #region Constructors

        public SupportsAsyncBulkUpdate()
            : this(LockFactory.Current.CreateAsyncLock())
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateLock">Lock to be acquired when update is in progress. When specified, only one bulk update at the time can happen. NULL if no lock to acquire, this allows muliple concurrent bulk updates (but risks them overriding each other)</param>
        public SupportsAsyncBulkUpdate(IAsyncLock updateLock)
        {
            _lock = updateLock;
        }

        #endregion

        public bool IsBulkUpdateInProgress()
        {
            return BulkUpdateCount != 0;
        }

        #region BeginBulkUpdateAsync (overloads)
        
        public async Task<IBulkUpdate> BeginBulkUpdateAsync(bool continueOnCapturedContext = false)
        {
            return
                await
                BeginBulkUpdateAsync(Timeout.Infinite, CancellationToken.None, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
        }

        public async Task<IBulkUpdate> BeginBulkUpdateAsync(CancellationToken ct, bool continueOnCapturedContext = false)
        {
            return
                await
                BeginBulkUpdateAsync(Timeout.Infinite, ct, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
        }

        public async Task<IBulkUpdate> BeginBulkUpdateAsync(int millisecondsTimeout, bool continueOnCapturedContext = false)
        {
            return
                await
                BeginBulkUpdateAsync(millisecondsTimeout, CancellationToken.None, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
        }
        
        public async Task<IBulkUpdate> BeginBulkUpdateAsync(TimeSpan timeout, bool continueOnCapturedContext = false)
        {
            return
                await
                BeginBulkUpdateAsync((int)timeout.TotalMilliseconds, CancellationToken.None, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
        }

        public async Task<IBulkUpdate> BeginBulkUpdateAsync(TimeSpan timeout, CancellationToken ct, bool continueOnCapturedContext = false)
        {
            return
                await
                BeginBulkUpdateAsync((int)timeout.TotalMilliseconds, ct, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
        }

        #endregion

        public async Task<IBulkUpdate> BeginBulkUpdateAsync(
            int millisecondsTimeout, 
            CancellationToken ct, 
            bool continueOnCapturedContext = false)
        {
            OnBeforeBeginBulkUpdate();

            var la = (ILockAcquisition)null;

            if (Lock != null)
            {
                la =
                    await
                    Lock
                    .AcqureWriteLockAsync(millisecondsTimeout, ct)
                    .ConfigureAwait(continueOnCapturedContext);

                if (!la.IsLockHeld)
                {
                    // failed to acquire lock
                    return new _FailedBulkUpdate();
                }
            }

            Interlocked.Increment(ref BulkUpdateCount);

            OnAfterBeginBulkUpdate();

            return new _BulkUpdate(la, DisposableObject.Create(() =>
            {
                EndBulkUpdate();
            }));
        }

        void EndBulkUpdate()
        {
            OnBeforeEndBulkUpdate();

            var _count = Interlocked.Decrement(ref BulkUpdateCount);
            
            if (_count == 0)
            {
                IncrementVersion();
                OnAfterEndBulkUpdate();
            }
        }

        /// <summary>
        /// Occurs when bulk update start has been requested but not yet started.
        /// </summary>
        protected void OnBeforeBeginBulkUpdate()
        { }

        /// <summary>
        /// Occurs when bulk update has started.
        /// </summary>
        protected void OnAfterBeginBulkUpdate()
        { }

        /// <summary>
        /// Occurs when update end has been requested but update not ended yet.
        /// </summary>
        protected virtual void OnBeforeEndBulkUpdate()
        { }

        /// <summary>
        /// Occurs when update has ended.
        /// </summary>
        protected virtual void OnAfterEndBulkUpdate()
        { }
    }
}
