using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation
{
    [DebuggerDisplay("WorkItem {Id}")]
    public class InvocationThrottleWorkItem
    {
        public Guid Id { get; } = Guid.NewGuid();

        public DateTime? CompleteTimeUtc { get; internal set; }
        public DateTime RequestWindowTimeUTC { get; internal set; }
        public DateTime RequestTimeUTC { get; internal set; }
        public object State { get; internal set; }
        public Task Task { get; internal set; }
        public bool MustRunToCompletion { get; internal set; }
        public CancellationTokenSource CancellationTokenSource { get; internal set; }
        internal Action<object, InvocationThrottleWorkItem, CancellationToken> Action { get; set; }
    }
}
