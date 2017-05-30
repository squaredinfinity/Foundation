using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public interface IRetryPolicyOptions
    {
        int MaxRetryAttempts { get; }
        int MinRetryDelayInMiliseconds { get; }
        int MaxRetryDelayInMiliseconds { get; }
        IReadOnlyList<ITransientFaultFilter> TransientFaultFilters { get; }
    }
}
