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

        static RetryPolicy()
        {
            var dp = new RetryPolicy();

            // treat all exceptions as transient by default
            dp.DefaultTransientFaultFilters.Add(new DynamicTransientFaultFilter<Exception>(ex => true));

            _defaultPolicy = dp;
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
            },
                Default_MaxRetryAttempts,
                Default_MinRetryDelayInMiliseconds,
                Default_MaxRetryDelayInMiliseconds,
                transientFaultFilters);
        }

        public TResult Execute<TResult>(Func<TResult> func)
        {
            return Execute(
                func,
                Default_MaxRetryAttempts,
                Default_MinRetryDelayInMiliseconds,
                Default_MaxRetryDelayInMiliseconds,
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

            while (!success && maxRetryAttempts--> 0)
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
                throw new AggregateException(
                    "Operation failed to execute {0} times."
                    .FormatWith(
                    failedAttempts.Count),
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
