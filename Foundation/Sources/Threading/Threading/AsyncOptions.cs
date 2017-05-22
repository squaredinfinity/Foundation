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
            _timeout = new TimeoutExpiry(millisecondsTimeout);
            CancellationToken = ct;
            ContinueOnCapturedContext = continueOnCapturedContext;
        }
    }
}
