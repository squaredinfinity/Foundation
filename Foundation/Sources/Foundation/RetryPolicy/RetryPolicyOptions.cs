using System;
using SquaredInfinity.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public class RetryPolicyOptions : IRetryPolicyOptions
    {
        public static IRetryPolicyOptions Default { get; private set; }
        public static void SetDefault(IRetryPolicyOptions newDefault)
        {
            Default = newDefault;
        }
        static RetryPolicyOptions()
        {
            Default = new RetryPolicyOptions
            {
                MaxRetryAttempts = 10,
                MinRetryDelayInMiliseconds = 50,
                MaxRetryDelayInMiliseconds = 100
            };
        }


        public int MaxRetryAttempts { get; set; }
        public int MinRetryDelayInMiliseconds { get; set; }
        public int MaxRetryDelayInMiliseconds { get; set; }
        public List<ITransientFaultFilter> TransientFaultFilters { get; set; } = new List<ITransientFaultFilter>();

        IReadOnlyList<ITransientFaultFilter> IRetryPolicyOptions.TransientFaultFilters
        {
            get { return TransientFaultFilters; }
        }

        public RetryPolicyOptions()
        {
            MaxRetryAttempts = Default.MaxRetryAttempts;
            MinRetryDelayInMiliseconds = Default.MinRetryDelayInMiliseconds;
            MaxRetryDelayInMiliseconds = Default.MaxRetryDelayInMiliseconds;

            TransientFaultFilters.AddRange(Default.TransientFaultFilters.EmptyIfNull());
        }
    }
}
