using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Comparers;

namespace SquaredInfinity.Foundation
{
    // retry transient faults only
    // if non-transient fault occurs, break operation
    // if failed after x attempts -> throw aggregate exception

    public class RetryPolicy : IRetryPolicy
    {
        public static class KnownTransientFaultFilters
        {
            public static ITransientFaultFilter AnyException = new DynamicTransientFaultFilter<Exception>(ex => true);
            public static ITransientFaultFilter FileAccessDenied = new DynamicTransientFaultFilter<IOException>(ex => ex.HResult == -2147024864);
        }

        Random Rand = new Random();

        IRetryPolicyOptions _defaultOptions = new RetryPolicyOptions();
        public IRetryPolicyOptions DefaultOptions
        {
            get { return _defaultOptions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                _defaultOptions = value;
            }
        }

        public static RetryPolicy Default { get; set; }
        public static RetryPolicy NoRetry { get; set; }
        public static RetryPolicy FileAccess { get; set; }

        #region Constructors

        public RetryPolicy()
            : this(new RetryPolicyOptions())
        { }

        public RetryPolicy(IRetryPolicyOptions options)
        {
            DefaultOptions = options;
        }

        static RetryPolicy()
        {
            //# set default policy
            var options = new RetryPolicyOptions();
            options.TransientFaultFilters.Add(KnownTransientFaultFilters.AnyException);

            var dp = new RetryPolicy(options);
            Default = dp;


            //# set no-retry policy
            options = new RetryPolicyOptions();
            options.MaxRetryAttempts = 0;

            dp = new RetryPolicy(options);
            NoRetry = dp;


            //# set default file access policy
            options = new RetryPolicyOptions();
            options.TransientFaultFilters.Add(KnownTransientFaultFilters.FileAccessDenied);
            dp = new RetryPolicy(options);
            FileAccess = dp;
        }

        #endregion

        #region Execute Action

        public void Execute(Action action)
        {
            Execute(DefaultOptions, action);
        }

        public void Execute(
            IRetryPolicyOptions options,
            Action action)
        {
            ExecuteAsyncInternal(
                () =>
                {
                    action();
                    return Task.FromResult(true);
                }, options)
                .Wait(options.CancellationToken);
        }

        public async Task ExecuteAsync(Action action)
        {
            await 
                ExecuteAsync(DefaultOptions, action)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task ExecuteAsync(IRetryPolicyOptions options, Action action)
        {
            await
                ExecuteAsync(
                    options,
                    () =>
                    {
                        action();
                        return Task.FromResult(true);

                    })
                    .ConfigureAwait(continueOnCapturedContext: false);
        }

        #endregion

        #region Execute Func

        public TResult Execute<TResult>(Func<TResult> func)
        {
            return Execute(DefaultOptions, func);
        }

        public TResult Execute<TResult>(
            IRetryPolicyOptions options,
            Func<TResult> func)
        {
            return 
                ExecuteAsyncInternal(
                    () => Task.FromResult(func()), options)
                    .Result;
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> func)
        {
            return
                await
                ExecuteAsync<TResult>(DefaultOptions, func)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<TResult> ExecuteAsync<TResult>(
            IRetryPolicyOptions options,
            Func<Task<TResult>> func)
        {

            return
                await
                ExecuteAsyncInternal<TResult>(func, options)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        async Task<TResult> ExecuteAsyncInternal<TResult>(
            Func<Task<TResult>> func,
            IRetryPolicyOptions options)
        { 
            var result = default(TResult);

            List<Exception> failedAttempts = new List<Exception>(capacity: options.MaxRetryAttempts);
            var max_retry_attempts = options.MaxRetryAttempts;

            bool success = false;

            while (!success && max_retry_attempts-- >= 0)
            {
                options.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    result =
                        await 
                        func()
                        .ConfigureAwait(continueOnCapturedContext: false); ;

                    success = true;
                }
                catch (Exception ex)
                {
                    var transient_fault_filters = options.TransientFaultFilters;

                    if (transient_fault_filters.Count == 0)
                        throw;

                    for (int i = 0; i < transient_fault_filters.Count; i++)
                    {
                        var filter = transient_fault_filters[i];

                        if (!filter.IsTransientFault(ex))
                            throw;

                        failedAttempts.Add(ex);

                        var retryDelay = Rand.Next(options.MinRetryDelayInMiliseconds, options.MaxRetryDelayInMiliseconds);

                        await 
                            Task.Delay(retryDelay, options.CancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);
                    }
                }
            }

            if (!success)
            {
                throw new AggregateException(
                    $"Operation failed to execute {failedAttempts.Count} times.",
                    failedAttempts.Distinct(ExceptionEqualityComparer.Default).ToArray());
            }

            return result;
        }

        #endregion
    }
}



//[TestClass]
//public class FileAccessTests
//{
//    [TestMethod]
//    public void MyTestMethod()
//    {
//        int count = 500;

//        var defp = new RetryPolicy();

//        defp.DefaultTransientFaultFilters.Add(
//            new DynamicTransientFaultFilter<IOException>(ex => true));

//        RetryPolicy.SetDefaultPolict(defp);

//        Parallel.For(0, 1000, (x) =>
//        {
//            if (x % 2 == 0)
//            {
//                string s = null;
//                if (!TryRead(out s))
//                {
//                    Trace.WriteLine("Failed Read");
//                }
//                else
//                {
//                    Trace.WriteLine("Read " + s);
//                }
//            }
//            else
//            {
//                var local_count = Interlocked.Decrement(ref count);

//                var s = local_count + " : " + new string((char)new Random().Next('a', 'z'), local_count);

//                if (!TryWrite(s))
//                {
//                    Trace.WriteLine("Failed Write");
//                }
//                else
//                {
//                    Trace.WriteLine("Wrote " + s);
//                }
//            }
//        });
//    }

//    public bool TryRead(out string txt)
//    {
//        if (File.Exists("1.txt"))
//        {
//            var localTxt = (string)null;

//            if (RetryPolicy.Default.Execute(() =>
//            {
//                // get read lock on a file
//                using (var fs = File.Open("1.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
//                {
//                    using (var sr = new StreamReader(fs))
//                    {
//                        localTxt = sr.ReadToEnd();
//                        return true;
//                    }
//                }
//            }))
//            {
//                txt = localTxt;
//                return true;
//            }
//        }

//        txt = null;
//        return false;
//    }

//    public bool TryWrite(string txt)
//    {
//        return RetryPolicy.Default.Execute(() =>
//        {
//            // get write lock on a file
//            using (var fs = File.Open("1.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
//            {
//                fs.SetLength(0);

//                using (var sw = new StreamWriter(fs))
//                {
//                    sw.Write(txt);
//                    return true;
//                }
//            }
//        });
//    }
//}
