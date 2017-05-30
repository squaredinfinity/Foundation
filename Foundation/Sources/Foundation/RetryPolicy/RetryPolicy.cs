using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using SquaredInfinity.Comparers;
using SquaredInfinity.Threading;

namespace SquaredInfinity
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

        #region Execute Action (overloads)

        public void Execute(Action action)
            => Execute(DefaultOptions, SyncOptions.Default, action);
        public void Execute(CancellationToken ct, Action action)
            => Execute(DefaultOptions, new SyncOptions(ct), action);
        public void Execute(TimeSpan timeout, Action action)
            => Execute(DefaultOptions, new SyncOptions(timeout), action);
        public void Execute(TimeSpan timeout, CancellationToken ct, Action action)
            => Execute(DefaultOptions, new SyncOptions(timeout, ct), action);
        public void Execute(int millisecondsTimeout, Action action)
            => Execute(DefaultOptions, new SyncOptions(millisecondsTimeout), action);
        public void Execute(int millisecondsTimeout, CancellationToken ct, Action action)
            => Execute(DefaultOptions, new SyncOptions(millisecondsTimeout, ct), action);
        public void Execute(SyncOptions syncOptions, Action action)
            => Execute(DefaultOptions, syncOptions, action);

        public void Execute(IRetryPolicyOptions options, Action action) => Execute(options, SyncOptions.Default, action);
        public void Execute(IRetryPolicyOptions options, CancellationToken ct, Action action)
             => Execute(options, new SyncOptions(ct), action);
        public void Execute(IRetryPolicyOptions options, TimeSpan timeout, Action action)
            => Execute(options, new SyncOptions(timeout), action);
        public void Execute(IRetryPolicyOptions options, TimeSpan timeout, CancellationToken ct, Action action)
            => Execute(options, new SyncOptions(timeout, ct), action);
        public void Execute(IRetryPolicyOptions options, int millisecondsTimeout, Action action)
            => Execute(options, new SyncOptions(millisecondsTimeout), action);
        public void Execute(IRetryPolicyOptions options, int millisecondsTimeout, CancellationToken ct, Action action)
            => Execute(options, new SyncOptions(millisecondsTimeout, ct), action);

        #endregion

        #region Execute Action Async (overloads)

        public async Task ExecuteAsync(Action action)
            => await ExecuteAsync(DefaultOptions, AsyncOptions.Default, action);
        public async Task ExecuteAsync(CancellationToken ct, Action action)
            => await ExecuteAsync(DefaultOptions, new AsyncOptions(ct), action);
        public async Task ExecuteAsync(TimeSpan timeout, Action action)
            => await ExecuteAsync(DefaultOptions, new AsyncOptions(timeout), action);
        public async Task ExecuteAsync(TimeSpan timeout, CancellationToken ct, Action action)
            => await ExecuteAsync(DefaultOptions, new AsyncOptions(timeout, ct), action);
        public async Task ExecuteAsync(int millisecondsTimeout, Action action)
            => await ExecuteAsync(DefaultOptions, new AsyncOptions(millisecondsTimeout), action);
        public async Task ExecuteAsync(int millisecondsTimeout, CancellationToken ct, Action action)
            => await ExecuteAsync(DefaultOptions, new AsyncOptions(millisecondsTimeout, ct), action);
        public async Task ExecuteAsync(AsyncOptions asyncOptions, Action action)
            => await ExecuteAsync(DefaultOptions, asyncOptions, action);

        public async Task ExecuteAsync(IRetryPolicyOptions options, Action action)
            => await ExecuteAsync(options, AsyncOptions.Default, action);
        public async Task ExecuteAsync(IRetryPolicyOptions options, CancellationToken ct, Action action)
            => await ExecuteAsync(options, new AsyncOptions(ct), action);
        public async Task ExecuteAsync(IRetryPolicyOptions options, TimeSpan timeout, Action action)
            => await ExecuteAsync(options, new AsyncOptions(timeout), action);
        public async Task ExecuteAsync(IRetryPolicyOptions options, TimeSpan timeout, CancellationToken ct, Action action)
            => await ExecuteAsync(options, new AsyncOptions(timeout, ct), action);
        public async Task ExecuteAsync(IRetryPolicyOptions options, int millisecondsTimeout, Action action)
            => await ExecuteAsync(options, new AsyncOptions(millisecondsTimeout), action);
        public async Task ExecuteAsync(IRetryPolicyOptions options, int millisecondsTimeout, CancellationToken ct, Action action)
            => await ExecuteAsync(options, new AsyncOptions(millisecondsTimeout, ct), action);

        #endregion

        #region Execute Func (overloads)

        public TResult Execute<TResult>(Func<TResult> func)
            => Execute(DefaultOptions, SyncOptions.Default, func);
        public TResult Execute<TResult>(CancellationToken ct, Func<TResult> func)
            => Execute(DefaultOptions, new SyncOptions(ct), func);
        public TResult Execute<TResult>(TimeSpan timeout, Func<TResult> func)
            => Execute(DefaultOptions, new SyncOptions(timeout), func);
        public TResult Execute<TResult>(TimeSpan timeout, CancellationToken ct, Func<TResult> func)
            => Execute(DefaultOptions, new SyncOptions(timeout, ct), func);
        public TResult Execute<TResult>(int millisecondsTimeout, Func<TResult> func)
            => Execute(DefaultOptions, new SyncOptions(millisecondsTimeout), func);
        public TResult Execute<TResult>(int millisecondsTimeout, CancellationToken ct, Func<TResult> func)
            => Execute(DefaultOptions, new SyncOptions(millisecondsTimeout, ct), func);
        public TResult Execute<TResult>(SyncOptions syncOptions, Func<TResult> func)
            => Execute(DefaultOptions, syncOptions, func);

        public TResult Execute<TResult>(IRetryPolicyOptions options, Func<TResult> func)
            => Execute(options, SyncOptions.Default, func);
        public TResult Execute<TResult>(IRetryPolicyOptions options, CancellationToken ct, Func<TResult> func)
            => Execute(options, new SyncOptions(ct), func);
        public TResult Execute<TResult>(IRetryPolicyOptions options, TimeSpan timeout, Func<TResult> func)
            => Execute(options, new SyncOptions(timeout), func);
        public TResult Execute<TResult>(IRetryPolicyOptions options, TimeSpan timeout, CancellationToken ct, Func<TResult> func)
            => Execute(options, new SyncOptions(timeout, ct), func);
        public TResult Execute<TResult>(IRetryPolicyOptions options, int millisecondsTimeout, Func<TResult> func)
            => Execute(options, new SyncOptions(millisecondsTimeout), func);
        public TResult Execute<TResult>(IRetryPolicyOptions options, int millisecondsTimeout, CancellationToken ct, Func<TResult> func)
            => Execute(options, new SyncOptions(millisecondsTimeout, ct), func);

        #endregion

        #region Execute Func Async (overloads)

        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(DefaultOptions, AsyncOptions.Default, func);
        public async Task<TResult> ExecuteAsync<TResult>(CancellationToken ct, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(DefaultOptions, new AsyncOptions(ct), func);
        public async Task<TResult> ExecuteAsync<TResult>(TimeSpan timeout, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(DefaultOptions, new AsyncOptions(timeout), func);
        public async Task<TResult> ExecuteAsync<TResult>(TimeSpan timeout, CancellationToken ct, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(DefaultOptions, new AsyncOptions(timeout, ct), func);
        public async Task<TResult> ExecuteAsync<TResult>(int millisecondsTimeout, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(DefaultOptions, new AsyncOptions(millisecondsTimeout), func);
        public async Task<TResult> ExecuteAsync<TResult>(int millisecondsTimeout, CancellationToken ct, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(DefaultOptions, new AsyncOptions(millisecondsTimeout, ct), func);
        public async Task<TResult> ExecuteAsync<TResult>(AsyncOptions asyncOptions, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(DefaultOptions, asyncOptions, func);
        public async Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(options, AsyncOptions.Default, func);
        public async Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, CancellationToken ct, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(options, new AsyncOptions(ct), func);
        public async Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, TimeSpan timeout, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(options, new AsyncOptions(timeout), func);
        public async Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, TimeSpan timeout, CancellationToken ct, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(options, new AsyncOptions(timeout, ct), func);
        public async Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, int millisecondsTimeout, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(options, new AsyncOptions(millisecondsTimeout), func);
        public async Task<TResult> ExecuteAsync<TResult>(IRetryPolicyOptions options, int millisecondsTimeout, CancellationToken ct, Func<Task<TResult>> func)
            => await ExecuteAsync<TResult>(options, new AsyncOptions(millisecondsTimeout, ct), func);

        #endregion

        #region Execute Action

        public void Execute(
            IRetryPolicyOptions options,
            SyncOptions syncOptions,
            Action action)
        {
            var ok =
            ExecuteAsyncInternal(
                () =>
                {
                    action();
                    return Task.FromResult(true);
                }, 
                options,
                new AsyncOptions(syncOptions.MillisecondsTimeout, syncOptions.CancellationToken))
                .Wait(syncOptions.MillisecondsTimeout, syncOptions.CancellationToken);

            if (!ok)
                throw new TimeoutException();
        }

        public async Task ExecuteAsync(IRetryPolicyOptions options, AsyncOptions asyncOptions, Action action)
        {
            await
                ExecuteAsyncInternal(
                    () =>
                    {
                        action();
                        return Task.FromResult(true);

                    },
                    options,
                    asyncOptions)
                    .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
        }

        #endregion

        #region Execute Func

        public TResult Execute<TResult>(
            IRetryPolicyOptions options,
            SyncOptions syncOptions,
            Func<TResult> func)
        {
            return
                ExecuteAsyncInternal(
                    () => Task.FromResult(func()), options, new AsyncOptions(syncOptions.MillisecondsTimeout, syncOptions.CancellationToken))
                    .Result;
        }

        public async Task<TResult> ExecuteAsync<TResult>(
            IRetryPolicyOptions options,
            AsyncOptions asyncoptions,
            Func<Task<TResult>> func)
        {
            return
                await
                ExecuteAsyncInternal<TResult>(func, options, asyncoptions)
                .ConfigureAwait(asyncoptions.ContinueOnCapturedContext);
        }

        async Task<TResult> ExecuteAsyncInternal<TResult>(
            Func<Task<TResult>> func,
            IRetryPolicyOptions options,
            AsyncOptions asyncOptions)
        { 
            var result = default(TResult);

            List<Exception> failedAttempts = new List<Exception>(capacity: options.MaxRetryAttempts);
            var max_retry_attempts = options.MaxRetryAttempts;

            bool success = false;

            while (!success && max_retry_attempts-- >= 0)
            {
                asyncOptions.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    result =
                        await 
                        func()
                        .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);

                    success = true;
                }
                catch (Exception ex)
                {
                    var transient_fault_filters = options.TransientFaultFilters;

                    if (transient_fault_filters.Count == 0)
                        throw;

                    for (int i = 0; i < transient_fault_filters.Count; i++)
                    {
                        asyncOptions.CancellationToken.ThrowIfCancellationRequested();

                        var filter = transient_fault_filters[i];

                        if (!filter.IsTransientFault(ex))
                            throw;

                        failedAttempts.Add(ex);

                        var retryDelay = Rand.Next(options.MinRetryDelayInMiliseconds, options.MaxRetryDelayInMiliseconds);

                        await 
                            Task.Delay(retryDelay, asyncOptions.CancellationToken)
                            .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
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
