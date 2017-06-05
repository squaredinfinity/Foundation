using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public class SyncOptions
    {
        static SyncOptions _default;
        public static SyncOptions Default => _default;
        public static void SetDefault(SyncOptions options)
        {
            _default = options;
        }

        static SyncOptions()
        {
            SetDefault(new SyncOptions(System.Threading.Timeout.Infinite, CancellationToken.None));
        }

        ICorrelationToken _correlationToken;
        public ICorrelationToken CorrelationToken
        {
            get { return _correlationToken; }
            private set { _correlationToken = value; }
        }

        public SyncOptions CorrelateThisThread()
        {
            return new SyncOptions(MillisecondsTimeout, CancellationToken,  new ThreadCorrelationToken(Environment.CurrentManagedThreadId));
        }

        public SyncOptions Correlate(ICorrelationToken ICorrelationToken)
        {
            return new SyncOptions(MillisecondsTimeout, CancellationToken, ICorrelationToken);
        }


        TimeoutExpiry _timeout;
        public int MillisecondsTimeout => _timeout.MillisecondsLeft;

        public CancellationToken CancellationToken { get; private set; }

        #region Constructors

        public SyncOptions()
            : this(Default.MillisecondsTimeout, Default.CancellationToken) { }

        public SyncOptions(ICorrelationToken ICorrelationToken)
            : this(Default.MillisecondsTimeout, Default.CancellationToken, ICorrelationToken) { }

        public SyncOptions(AsyncOptions options)
            : this(options.MillisecondsTimeout, options.CancellationToken) { }

        public SyncOptions(CancellationToken ct)
            : this(Default.MillisecondsTimeout, ct) { }

        public SyncOptions(CancellationToken ct, ICorrelationToken ICorrelationToken)
            : this(Default.MillisecondsTimeout, ct, ICorrelationToken) { }

        public SyncOptions(TimeSpan timeout)
            : this((int)timeout.TotalMilliseconds, Default.CancellationToken) { }

        public SyncOptions(TimeSpan timeout, ICorrelationToken ICorrelationToken)
            : this((int)timeout.TotalMilliseconds, Default.CancellationToken, ICorrelationToken) { }

        public SyncOptions(TimeSpan timeout, CancellationToken ct)
            : this((int)timeout.TotalMilliseconds, ct) { }

        public SyncOptions(TimeSpan timeout, CancellationToken ct, ICorrelationToken ICorrelationToken)
            : this((int)timeout.TotalMilliseconds, ct, ICorrelationToken) { }

        public SyncOptions(int millisecondsTmeout)
            : this(millisecondsTmeout, Default.CancellationToken) { }

        public SyncOptions(int millisecondsTmeout, ICorrelationToken ICorrelationToken)
            : this(millisecondsTmeout, Default.CancellationToken, ICorrelationToken) { }

        public SyncOptions(int millisecondsTimeout, CancellationToken ct)
            : this(millisecondsTimeout, ct, null) { }

        public SyncOptions(int millisecondsTimeout, CancellationToken ct, ICorrelationToken ICorrelationToken)
        {
            _correlationToken = ICorrelationToken;
            _timeout = new TimeoutExpiry(millisecondsTimeout);
            CancellationToken = ct;
        }

        #endregion

        public AsyncOptions ToAsync()
        {
            return new AsyncOptions(MillisecondsTimeout, CancellationToken, CorrelationToken, AsyncOptions.Default.ContinueOnCapturedContext);
        }

        public AsyncOptions ToAsync(bool continueOnCapturedContext)
        {
            return new AsyncOptions(MillisecondsTimeout, CancellationToken, CorrelationToken, continueOnCapturedContext);
        }
    }
}
