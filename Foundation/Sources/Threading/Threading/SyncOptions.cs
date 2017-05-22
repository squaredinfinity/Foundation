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

        TimeoutExpiry _timeout;
        public int MillisecondsTimeout => _timeout.MillisecondsLeft;

        public CancellationToken CancellationToken { get; private set; }

        public SyncOptions()
            : this(System.Threading.Timeout.Infinite, CancellationToken.None) { }

        public SyncOptions(CancellationToken ct)
            : this(System.Threading.Timeout.Infinite, ct) { }

        public SyncOptions(TimeSpan timeout)
            : this((int)timeout.TotalMilliseconds, CancellationToken.None) { }

        public SyncOptions(TimeSpan timeout, CancellationToken ct)
            : this((int)timeout.TotalMilliseconds, ct) { }

        public SyncOptions(int millisecondsTmeout)
            : this(millisecondsTmeout, CancellationToken.None) { }

        public SyncOptions(int millisecondsTimeout, CancellationToken ct)
        {
            _timeout = new TimeoutExpiry(millisecondsTimeout);
            CancellationToken = ct;
        }
    }
}
