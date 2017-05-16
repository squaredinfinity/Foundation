using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Threading.Locks
{
    public class SemaphoreSlimEx : CompositeLock, IAsyncLock, ICompositeLock
    {
        readonly internal SemaphoreSlim InternalWriteLock = new SemaphoreSlim(1, 1);

        public Guid UniqueId { get; } = Guid.NewGuid();

        public string Name { get; private set; }

        class WriteLockAcquisition : DisposableObject, IWriteLockAcquisition
        {
            SemaphoreSlimEx Owner;

            public WriteLockAcquisition(SemaphoreSlimEx owner, IDisposable disposeWhenDone)
            {
                this.Owner = owner;

                Disposables.AddIfNotNull(disposeWhenDone);
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                Owner.InternalWriteLock.Release();
            }
        }


        public async Task<IWriteLockAcquisition> AcqureWriteLockAsync()
        {
            // lock parent first
            await InternalWriteLock.WaitAsync();
            // then its children
            //var disposables = LockChildren(LockModes.Write);

            return 
                new WriteLockAcquisition(owner: this, disposeWhenDone: (IDisposable)null);
        }

        public async Task<IWriteLockAcquisition> AcqureWriteLockAsync(CancellationToken ct)
        {
            // lock parent first
            await InternalWriteLock.WaitAsync(ct);
            // then its children
            //var disposables = LockChildren(LockModes.Write);

            return
                new WriteLockAcquisition(owner: this, disposeWhenDone: (IDisposable)null);
        }

        public Task<IWriteLockAcquisition> AcqureWriteLockAsync(int millisecondsTimeout)
        {
            throw new NotImplementedException();
        }

        public Task<IWriteLockAcquisition> AcqureWriteLockAsync(int millisecondsTimeout, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IWriteLockAcquisition> AcqureWriteLockAsync(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<IWriteLockAcquisition> AcqureWriteLockAsync(TimeSpan timeout, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
