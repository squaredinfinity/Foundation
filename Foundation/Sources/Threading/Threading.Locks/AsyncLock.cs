﻿using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Threading.Locks
{
    public partial class AsyncLock : IAsyncLock, ICompositeAsyncLock
    {
        readonly _CompositeAsyncLock CompositeLock;

        readonly internal SemaphoreSlim InternalWriteLock = new SemaphoreSlim(1, 1);

        public long LockId { get; } = _LockIdProvider.GetNextId();
        public string Name { get; private set; }

        #region Constructors

        public AsyncLock()
            : this(name: "", supportsComposition: true)
        { }

        public AsyncLock(string name)
            : this(name, supportsComposition: true)
        { }

        public AsyncLock(bool supportsComposition)
            : this("", supportsComposition)
        { }

        public AsyncLock(string name, bool supportsComposition)
        {
            Name = name;

            if (supportsComposition)
                CompositeLock = new _CompositeAsyncLock();
            else
                CompositeLock = null;
        }

        #endregion

        #region Acquire Write Lock Async (overloads)

        public async Task<ILockAcquisition> AcqureWriteLockAsync(bool continueOnCapturedContext = false)
        {
            return
                await
                AcqureWriteLockAsync(Timeout.Infinite, CancellationToken.None, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
        }

        public async Task<ILockAcquisition> AcqureWriteLockAsync(CancellationToken ct, bool continueOnCapturedContext = false)
        {
            return
                await
                AcqureWriteLockAsync(Timeout.Infinite, ct, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
        }

        public async Task<ILockAcquisition> AcqureWriteLockAsync(int millisecondsTimeout, bool continueOnCapturedContext = false)
        {
            return
                await
                AcqureWriteLockAsync(millisecondsTimeout, CancellationToken.None, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
        }

        public async Task<ILockAcquisition> AcqureWriteLockAsync(TimeSpan timeout, bool continueOnCapturedContext = false)
        {
            return
                await
                AcqureWriteLockAsync((int)timeout.TotalMilliseconds, CancellationToken.None, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
        }

        public async Task<ILockAcquisition> AcqureWriteLockAsync(TimeSpan timeout, CancellationToken ct, bool continueOnCapturedContext = false)
        {
            return
                await
                AcqureWriteLockAsync((int)timeout.TotalMilliseconds, ct, continueOnCapturedContext)
                .ConfigureAwait(continueOnCapturedContext);
        }

        #endregion

        #region Acquire Write Lock Async

        public async Task<ILockAcquisition> AcqureWriteLockAsync(int millisecondsTimeout, CancellationToken ct, bool continueOnCapturedContext = false)
        {
            var dispose_when_done = (IDisposable)null;

            // lock parent first
            var ok =
                await
                InternalWriteLock
                .WaitAsync(millisecondsTimeout, ct)
                .ConfigureAwait(continueOnCapturedContext);

            if (!ok)
                return new _FailedLockAcquisition();

            try
            {
                // then its children
                if (CompositeLock != null)
                {
                    var children_acquisition =
                        await
                        CompositeLock.LockChildrenAsync(LockType.Write, millisecondsTimeout, ct, continueOnCapturedContext);

                    if (!children_acquisition.IsLockHeld)
                    {
                        // couldn't acquire children, release parent lock
                        InternalWriteLock.Release();
                    }
                }
            }
            catch
            {
                // some error occured, release parent lock
                InternalWriteLock.Release();

                throw;
            }

            return
                new _WriteLockAcquisition(owner: this, disposeWhenDone: dispose_when_done);

        }

        #endregion

        #region ICompositeAsyncLock

        public void AddChild(IAsyncLock childLock)
        {
            if (CompositeLock == null)
                throw new NotSupportedException("This lock does not support .AddChild()");


            CompositeLock.AddChild(childLock);
        }

        public void RemoveChild(IAsyncLock childLock)
        {
            if (CompositeLock == null)
                throw new NotSupportedException("This lock does not support .RemoveChild()");

            CompositeLock.RemoveChild(childLock);
        }

        #endregion
    }
}
