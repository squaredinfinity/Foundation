using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using System.Threading;

namespace SquaredInfinity.Foundation
{
    // retry transient faults only
    // if non-transient fault occurs, break operation
    // if failed after x attempts -> throw aggregate exception

    public class RetryPolicy
    {
        Random Rand = new Random();

        const int Default_MaxRetryAttempts = 10;
        const int Default_MinRetryDelayInMiliseconds = 50;
        const int Default_MaxRetryDelayInMiliseconds = 100;

        static RetryPolicy _defaultPolicy;
        public static RetryPolicy Default { get { return _defaultPolicy; } }

        public static void SetDefaultPolict(RetryPolicy newDefaultPolicy)
        {
            _defaultPolicy = newDefaultPolicy;
        }

        public List<ITransientFaultFilter> DefaultTransientFaultFilters { get; private set; }

        public RetryPolicy()
        {
            DefaultTransientFaultFilters = new List<ITransientFaultFilter>();
        }

        public void Execute(Action action)
        {
            Execute(action, null);
        }

        public void Execute(Action action, IReadOnlyList<ITransientFaultFilter> transientFaultFilters)
        {
            Execute(() =>
                {
                    action();
                    return true;
                }, transientFaultFilters: transientFaultFilters);
        }

        public TResult Execute<TResult>(Func<TResult> func)
        {
            return Execute(func);
        }

        public TResult Execute<TResult>(
            Func<TResult> func, 
            int maxRetryAttempts = Default_MaxRetryAttempts, 
            int minDelayInMiliseconds = Default_MinRetryDelayInMiliseconds, 
            int maxDelayInMiliseconds = Default_MaxRetryDelayInMiliseconds, 
            IReadOnlyList<ITransientFaultFilter> transientFaultFilters = null)
        {
            if (transientFaultFilters == null)
                transientFaultFilters = new List<ITransientFaultFilter>(capacity: 0);

            var result = default(TResult);

            List<Exception> failedAttempts = new List<Exception>(capacity: maxRetryAttempts);

            var filters = transientFaultFilters.EmptyIfNull().ToArray();

            bool success = false;

            while (!success && maxRetryAttempts --> 0)
            {
                try
                {
                    result = func();
                    success = true;
                }
                catch (Exception ex)
                {
                    for (int i = 0; i < transientFaultFilters.Count; i++)
                    {
                        var filter = transientFaultFilters[i];

                        if (!filter.IsTransientFault(ex))
                            throw;

                        failedAttempts.Add(ex);

                        TimeSpan retryDelay = 
                            TimeSpan.FromMilliseconds(
                                Rand.Next(minDelayInMiliseconds, 
                                maxDelayInMiliseconds));

                        Thread.Sleep(retryDelay);
                    }
                }
            }

            if (!success)
            {
                throw new AggregateException("Operation failed to execute {0} times.", failedAttempts);
            }

            return result;
        }
    }
}
