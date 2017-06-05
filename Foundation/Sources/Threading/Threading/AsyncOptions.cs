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

        #region Correlation ID

        ICorrelationToken _correlationToken;
        public ICorrelationToken CorrelationToken
        {
            get { return _correlationToken; }
            private set { _correlationToken = value; }
        }

        public AsyncOptions CorrelateThisThread()
        {
            return new AsyncOptions(MillisecondsTimeout, CancellationToken, new ThreadCorrelationToken(Environment.CurrentManagedThreadId));
        }

        public AsyncOptions Correlate(ICorrelationToken ICorrelationToken)
        {
            return new AsyncOptions(MillisecondsTimeout, CancellationToken, ICorrelationToken);
        }

        #endregion

        readonly TimeoutExpiry _timeout;
        public int MillisecondsTimeout => _timeout.MillisecondsLeft;
        
        public CancellationToken CancellationToken { get; private set; }
        public bool ContinueOnCapturedContext { get; }

        #region Constructors

        public AsyncOptions()
            : this(Default.MillisecondsTimeout, Default.CancellationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(ICorrelationToken ICorrelationToken)
            : this(Default.MillisecondsTimeout, Default.CancellationToken, ICorrelationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(SyncOptions options)
            : this(options.MillisecondsTimeout, options.CancellationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(bool continueOnCapturedContext)
            : this(Default.MillisecondsTimeout, Default.CancellationToken, continueOnCapturedContext) { }

        public AsyncOptions(bool continueOnCapturedContext, ICorrelationToken ICorrelationToken)
            : this(Default.MillisecondsTimeout, Default.CancellationToken, ICorrelationToken, continueOnCapturedContext) { }

        public AsyncOptions(CancellationToken ct)
            : this(Default.MillisecondsTimeout, ct, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(CancellationToken ct, ICorrelationToken ICorrelationToken)
            : this(Default.MillisecondsTimeout, ct, ICorrelationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(CancellationToken ct, bool continueOnCapturedContext)
            : this(Default.MillisecondsTimeout, ct, continueOnCapturedContext) { }

        public AsyncOptions(CancellationToken ct, ICorrelationToken ICorrelationToken, bool continueOnCapturedContext)
            : this(Default.MillisecondsTimeout, ct, ICorrelationToken, continueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout)
            : this((int)timeout.TotalMilliseconds, Default.CancellationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, ICorrelationToken ICorrelationToken)
            : this((int)timeout.TotalMilliseconds, Default.CancellationToken, ICorrelationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, bool continueOnCapturedContext)
            : this((int)timeout.TotalMilliseconds, Default.CancellationToken, continueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, bool continueOnCapturedContext, ICorrelationToken ICorrelationToken)
            : this((int)timeout.TotalMilliseconds, Default.CancellationToken, ICorrelationToken, continueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, CancellationToken ct)
            : this((int)timeout.TotalMilliseconds, ct, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, CancellationToken ct, ICorrelationToken ICorrelationToken)
            : this((int)timeout.TotalMilliseconds, ct, ICorrelationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, CancellationToken ct, bool continueOnCapturedContext)
            : this((int)timeout.TotalMilliseconds, ct, continueOnCapturedContext) { }

        public AsyncOptions(TimeSpan timeout, CancellationToken ct, bool continueOnCapturedContext, ICorrelationToken ICorrelationToken)
            : this((int)timeout.TotalMilliseconds, ct, ICorrelationToken, continueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTmeout)
            : this(millisecondsTmeout, Default.CancellationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTmeout, ICorrelationToken ICorrelationToken)
            : this(millisecondsTmeout, Default.CancellationToken, ICorrelationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTmeout, bool continueOnCapturedContext)
            : this(millisecondsTmeout, CancellationToken.None, continueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTmeout, bool continueOnCapturedContext, ICorrelationToken ICorrelationToken)
            : this(millisecondsTmeout, CancellationToken.None, ICorrelationToken, continueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTmeout, CancellationToken ct)
            : this(millisecondsTmeout, ct, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTmeout, CancellationToken ct, ICorrelationToken ICorrelationToken)
            : this(millisecondsTmeout, ct, ICorrelationToken, Default.ContinueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTimeout, CancellationToken ct, bool continueOnCapturedContext)
            : this(millisecondsTimeout, ct, null, continueOnCapturedContext) { }

        public AsyncOptions(int millisecondsTimeout, CancellationToken ct, ICorrelationToken ICorrelationToken, bool continueOnCapturedContext)
        {
            _correlationToken = ICorrelationToken;
            _timeout = new TimeoutExpiry(millisecondsTimeout);
            CancellationToken = ct;
            ContinueOnCapturedContext = continueOnCapturedContext;
        }

        #endregion

        public SyncOptions ToSync()
        {
            return new SyncOptions(MillisecondsTimeout, CancellationToken, CorrelationToken);
        }
    }
}
