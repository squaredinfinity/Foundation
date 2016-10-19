using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ParallelExtensions
    {
        //readonly static ILogger Diagnostics = InternalDiagnosticLogger.CreateLoggerForType(typeof(ParallelExtensions));

        static ParallelLoopResult CompletedResult;


        static Func<ParallelOptions> _optionsFactory = () =>
        {
            var result = new ParallelOptions();

#if DEBUG
            result.MaxDegreeOfParallelism = -1;
#else
            result.MaxDegreeOfParallelism = -1;
#endif

            return result;
        };

        public static void SetDefaultOptionsFactory(Func<ParallelOptions> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _optionsFactory = factory;
        }

        public static ParallelOptions GetDefaultParallelOptions(CancellationToken ct)
        {
            var op = _optionsFactory();
            op.CancellationToken = ct;

            return op;
        }

        public static ParallelOptions GetDisabledParallelOptions(CancellationToken ct)
        {
            var op = _optionsFactory();
            op.MaxDegreeOfParallelism = 1;
            op.CancellationToken = ct;

            return op;
        }

        static ParallelExtensions()
        {
            CompletedResult = CreateCompletedParallelLoopResult();
        }

        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            return ForEach(source, GetDefaultParallelOptions(CancellationToken.None), body);
        }

        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, ParallelOptions options, Action<TSource> body)
        {
            var collection = source as ICollection;

            if (collection != null)
            {
                if (collection.Count == 0)
                    return CompletedResult;

                if (collection.Count == 1)
                {
                    body(source.First());
                    return CompletedResult;
                }
            }

            return Parallel.ForEach(source, options, body);
        }

        public static void ForEachNoWait<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            Task.Run(() =>
            {
                Parallel.ForEach(source, item =>
                {
                    body(item);
                });
            });
        }

        /// <summary>
        /// ParallelLoopResult does not expose public setters.
        /// Must use reflection to create Completed Result without overhead of Parallel.XXX
        /// </summary>
        static ParallelLoopResult CreateCompletedParallelLoopResult()
        {
            var result = Parallel.For(0, 0, GetDefaultParallelOptions(CancellationToken.None), (i) => { });

            return result;
        }
    }
}
