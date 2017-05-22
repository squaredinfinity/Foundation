using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public partial class AsyncReaderWriterLock
    {
        public Task<ILockAcquisition> AcquireWriteLockAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
