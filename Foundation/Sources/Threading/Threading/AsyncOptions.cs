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
        static AsyncOptions _default;
        public static AsyncOptions Default => _default;
        static AsyncOptions _onCapturedContext;
        public static AsyncOptions OnCapturedContext => _onCapturedContext;

        public static void SetDefault(AsyncOptions options)
        {
            _default = options;
            _onCapturedContext =  new AsyncOptions(_default.MillisecondsTimeout, _default.CancellationToken, continueOnCapturedContext: true);
        }

        static AsyncOptions()
        {
            SetDefault(new AsyncOptions(System.Threading.Timeout.Infinite, CancellationToken.None, continueOnCapturedContext: false));
        }

        readonly TimeoutExpiry _timeout;
        public int MillisecondsTimeout => _timeout.MillisecondsLeft;
        
        public CancellationToken CancellationToken { get; private set; }
        public bool ContinueOnCapturedContext { get; }

        public AsyncOptions()
            : this(Default.MillisecondsTimeout, Default.CancellationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(SyncOptions options)
            : this(options.MillisecondsTimeout, options.CancellationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(bool continueOnCapturedContext)
            : this(Default.MillisecondsTimeout, Default.CancellationToken, continueOnCapturedContext) { }

        public AsyncOptions(CancellationToken ct)
            : this(Default.MillisecondsTimeout, ct, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(CancellationToken ct, bool continueOnCapturedContext)
            : this(Default.MillisecondsTimeout, ct, continueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout)
            : this((int)timeout.TotalMilliseconds, Default.CancellationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, bool continueOnCapturedContext)
            : this((int)timeout.TotalMilliseconds, Default.CancellationToken, continueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, CancellationToken ct)
            : this((int)timeout.TotalMilliseconds, ct, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, CancellationToken ct, bool continueOnCapturedContext)
            : this((int)timeout.TotalMilliseconds, ct, continueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTmeout)
            : this(millisecondsTmeout, Default.CancellationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTmeout, bool continueOnCapturedContext)
            : this(millisecondsTmeout, CancellationToken.None, continueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTmeout, CancellationToken ct)
            : this(millisecondsTmeout, ct, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTimeout, CancellationToken ct, bool continueOnCapturedContext)
        {
            _timeout = new TimeoutExpiry(millisecondsTimeout);
            CancellationToken = ct;
            ContinueOnCapturedContext = continueOnCapturedContext;
        }
    }
}
