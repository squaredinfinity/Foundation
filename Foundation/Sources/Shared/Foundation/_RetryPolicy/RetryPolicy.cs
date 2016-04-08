using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using System.Threading;
using System.IO;

namespace SquaredInfinity.Foundation
{
    // retry transient faults only
    // if non-transient fault occurs, break operation
    // if failed after x attempts -> throw aggregate exception

    public class RetryPolicy
    {
        public static class TransientFaultFilters
        {
            public static ITransientFaultFilter AnyException = new DynamicTransientFaultFilter<Exception>(ex => true);
            public static ITransientFaultFilter FileAccessDenied = new DynamicTransientFaultFilter<IOException>(ex => ex.HResult == -2147024864);
        }

        Random Rand = new Random();

        const int Default_MaxRetryAttempts = 10;
        const int Default_MinRetryDelayInMiliseconds = 50;
        const int Default_MaxRetryDelayInMiliseconds = 100;

        public int MaxRetryAttempts { get; private set; }
        public int MinRetryDelayInMiliseconds { get; private set; }
        public int MaxRetryDelayInMiliseconds { get; private set; }


        public static RetryPolicy Default { get; set; }

        public static RetryPolicy FileAccess { get; set; }

        public RetryPolicy()
            : this(Default_MaxRetryAttempts, TimeSpan.FromMilliseconds(Default_MinRetryDelayInMiliseconds), TimeSpan.FromMilliseconds(Default_MaxRetryDelayInMiliseconds))
        {
            DefaultTransientFaultFilters = new List<ITransientFaultFilter>();
        }

        public RetryPolicy(IReadOnlyList<ITransientFaultFilter> transientFaultFilters)
            : this(
                  Default_MaxRetryAttempts, 
                  TimeSpan.FromMilliseconds(Default_MinRetryDelayInMiliseconds),
                  TimeSpan.FromMilliseconds(Default_MaxRetryDelayInMiliseconds),
                  transientFaultFilters)
        { }

        public RetryPolicy(int maxRetryAttempts, TimeSpan minRetryDelay, TimeSpan maxRetryDelay)
            : this(
                  maxRetryAttempts, 
                  minRetryDelay, 
                  maxRetryDelay, 
                  new List<ITransientFaultFilter>())
        { }

        public RetryPolicy(
            int maxRetryAttempts, 
            TimeSpan minRetryDelay, 
            TimeSpan maxRetryDelay, 
            IReadOnlyList<ITransientFaultFilter> transientFaultFilters)
        {
            this.MaxRetryAttempts = maxRetryAttempts;
            this.MinRetryDelayInMiliseconds = (int)minRetryDelay.TotalMilliseconds;
            this.MaxRetryDelayInMiliseconds = (int)maxRetryDelay.TotalMilliseconds;

            var tff = new List<ITransientFaultFilter>();
            tff.AddRange(transientFaultFilters);

            DefaultTransientFaultFilters = tff;
        }

        static RetryPolicy()
        {

            //# set default policy
            var dp = new RetryPolicy(new[] { TransientFaultFilters.AnyException });
            Default = dp;


            //# set default file access policy
            dp = new RetryPolicy(new[] { TransientFaultFilters.FileAccessDenied });
            FileAccess = dp;
        }

        public IReadOnlyList<ITransientFaultFilter> DefaultTransientFaultFilters { get; private set; }

        public void Execute(Action action)
        {
            Execute(action, DefaultTransientFaultFilters);
        }

        public void Execute(
           Action action,
           int maxRetryAttempts,
           int minDelayInMiliseconds,
           int maxDelayInMiliseconds)
        {
            Execute(() =>
            {
                action();
                return true;
            },
                maxRetryAttempts,
                minDelayInMiliseconds,
                maxDelayInMiliseconds,
                DefaultTransientFaultFilters);
        }

        public void Execute(
           Action action,
           int maxRetryAttempts,
           int minDelayInMiliseconds,
           int maxDelayInMiliseconds,
           IReadOnlyList<ITransientFaultFilter> transientFaultFilters)
        {
            Execute(() =>
            {
                action();
                return true;
            },
                maxRetryAttempts,
                minDelayInMiliseconds,
                maxDelayInMiliseconds,
                transientFaultFilters);
        }

        public void Execute(Action action, IReadOnlyList<ITransientFaultFilter> transientFaultFilters)
        {
            Execute(() =>
            {
                action();
                return true;
            },
                MaxRetryAttempts,
                MinRetryDelayInMiliseconds,
                MaxRetryDelayInMiliseconds,
                transientFaultFilters);
        }

        public TResult Execute<TResult>(Func<TResult> func)
        {
            return Execute(
                func,
                MaxRetryAttempts,
                MinRetryDelayInMiliseconds,
                MaxRetryDelayInMiliseconds,
                DefaultTransientFaultFilters);
        }

        public TResult Execute<TResult>(
            Func<TResult> func,
            int maxRetryAttempts,
            int minDelayInMiliseconds,
            int maxDelayInMiliseconds)
        {
            return Execute<TResult>(
                func,
                maxRetryAttempts,
                minDelayInMiliseconds,
                maxDelayInMiliseconds,
                DefaultTransientFaultFilters);
        }

        public TResult Execute<TResult>(
            Func<TResult> func,
            int maxRetryAttempts,
            int minDelayInMiliseconds,
            int maxDelayInMiliseconds,
            IReadOnlyList<ITransientFaultFilter> transientFaultFilters)
        {
            if (transientFaultFilters == null)
                transientFaultFilters = new List<ITransientFaultFilter>(capacity: 0);

            var result = default(TResult);

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
                    if (transientFaultFilters.Count == 0)
                        throw;

                    for (int i = 0; i < transientFaultFilters.Count; i++)
                    {
                        var filter = transientFaultFilters[i];

                        if (!filter.IsTransientFault(ex))
                            throw;

                        failedAttempts.Add(ex);

                        var retryDelay = Rand.Next(minDelayInMiliseconds, maxDelayInMiliseconds);

                        Thread.Sleep(retryDelay);
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
