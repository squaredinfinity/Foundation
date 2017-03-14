using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public class RetryPolicyOptions : IRetryPolicyOptions
    {
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
        public int MaxRetryAttempts { get; set; } = 10;
        public int MinRetryDelayInMiliseconds { get; set; } = 50;
        public int MaxRetryDelayInMiliseconds { get; set; } = 100;
        public List<ITransientFaultFilter> TransientFaultFilters { get; set; } = new List<ITransientFaultFilter>();

        IReadOnlyList<ITransientFaultFilter> IRetryPolicyOptions.TransientFaultFilters
        {
            get { return TransientFaultFilters; }
        }
    }

    public interface IRetryPolicyOptions
    {
        CancellationToken CancellationToken { get; }
        int MaxRetryAttempts { get; }
        int MinRetryDelayInMiliseconds { get; }
        int MaxRetryDelayInMiliseconds { get; }
        IReadOnlyList<ITransientFaultFilter> TransientFaultFilters { get; }
    }
}
