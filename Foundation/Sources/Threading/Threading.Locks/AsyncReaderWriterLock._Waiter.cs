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
        class _Waiter
        {
            public readonly TaskCompletionSource<ILockAcquisition> TaskCompletionSource;
            public readonly TimeoutExpiry Timeout;
            public readonly CancellationToken CancellationToken;
            public readonly ICorrelationToken CorrelationToken;

            public _Waiter(
                TaskCompletionSource<ILockAcquisition> taskCompletionSource,
                ICorrelationToken correlationToken,
                int millisecondsTimeout, 
                CancellationToken ct)
            {
                TaskCompletionSource = taskCompletionSource;
                CorrelationToken = correlationToken;

                Timeout = new TimeoutExpiry(millisecondsTimeout);
                CancellationToken = ct;
            }
        }
    }
}
