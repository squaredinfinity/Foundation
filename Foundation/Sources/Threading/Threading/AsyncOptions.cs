using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public class AsyncOptions
    {
        public static readonly AsyncOptions Default = new AsyncOptions();
        public static readonly AsyncOptions OnCapturedContext = new AsyncOptions(continueOnCapturedContext: true);

        public int MillisecondsTimeout { get; private set; }
        public TimeSpan Timeout => TimeSpan.FromMilliseconds(MillisecondsTimeout);
        public CancellationToken CancellationToken { get; private set; }
        public bool ContinueOnCapturedContext { get; }

        public AsyncOptions()
            : this(System.Threading.Timeout.Infinite, CancellationToken.None, false) { }

        public AsyncOptions(bool continueOnCapturedContext = false)
            : this(System.Threading.Timeout.Infinite, CancellationToken.None, continueOnCapturedContext) { }

        public AsyncOptions(CancellationToken ct, bool continueOnCapturedContext = false)
            : this(System.Threading.Timeout.Infinite, ct, continueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, bool continueOnCapturedContext = false)
            : this((int)timeout.TotalMilliseconds, CancellationToken.None, continueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, CancellationToken ct, bool continueOnCapturedContext = false)
            : this((int)timeout.TotalMilliseconds, ct, continueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTmeout, bool continueOnCapturedContext = false)
            : this(millisecondsTmeout, CancellationToken.None, continueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTimeout, CancellationToken ct, bool continueOnCapturedContext = false)
        {
            MillisecondsTimeout = millisecondsTimeout;
            CancellationToken = ct;
            ContinueOnCapturedContext = continueOnCapturedContext;
        }
    }
}
