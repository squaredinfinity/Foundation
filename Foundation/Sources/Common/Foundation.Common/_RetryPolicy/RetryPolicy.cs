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

        public void Execute(Action action, params ITransientFaultFilter[] transientFaultFilters)
        {
            Execute(() =>
                {
                    action();
                    return true;
                });
        }

        public TResult Execute<TResult>(Func<TResult> func)
        {
            return Execute(func, null);
        }

        public TResult Execute<TResult>(Func<TResult> func, params ITransientFaultFilter[] transientFaultFilters)
        {
            var result = default(TResult);

            var maxRetryAttempts = 10;
            TimeSpan retryDelay = TimeSpan.FromMilliseconds(100);

            // todo: use circular queue instead
            List<Exception> failedAttempts = new List<Exception>(capacity: maxRetryAttempts);

            var filters = transientFaultFilters.EmptyIfNull().ToArray();

            bool success = false;

            while (!success && maxRetryAttempts-- > 0)
            {
                try
                {
                    result = func();
                    success = true;
                }
                catch (Exception ex)
                {
                    for (int i = 0; i < transientFaultFilters.Length; i++)
                    {
                        var filter = transientFaultFilters[i];

                        if (!filter.IsTransientFault(ex))
                            throw;

                        failedAttempts.Add(ex);

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
