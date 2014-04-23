using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ParallelExtensions
    {
        //readonly static ILogger Diagnostics = InternalDiagnosticLogger.CreateLoggerForType(typeof(ParallelExtensions));

        static ParallelLoopResult CompletedResult;

        static ParallelOptions _parallelOptions = new ParallelOptions();
        public static ParallelOptions DefaultParallelOptions
        {
            get { return _parallelOptions; }
        }

        static ParallelExtensions()
        {
            try
            {
                CompletedResult = CreateCompletedParallelLoopResult();
            }
            catch (Exception ex)
            {
                Diagnostics.Error(ex);
            }
        }

        public static ParallelLoopResult ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
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

            return Parallel.ForEach(source, body);
        }

        public static void ForEachNoWait<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            foreach (var item in source)
            {
                Task.Factory.StartNew(() => body(item));
            }
        }

        /// <summary>
        /// ParallelLoopResult does not expose public setters.
        /// Must use reflection to create Completed Result without overhead of Parallel.XXX
        /// </summary>
        static ParallelLoopResult CreateCompletedParallelLoopResult()
        {
            // TODO: test which one is quicker, reflection vs empty loop

            try
            {
                var result = CreateCompletedParallelLoopResultUsingReflection();
                return result;
            }
            catch (Exception ex)
            {
                // TODO
                //Diagnostics.Error(ex);

                // reflection failed, run empty loop to get completed result
                var result = Parallel.For(0, 0, (i) => { });

                return result;
            }
        }

        static ParallelLoopResult CreateCompletedParallelLoopResultUsingReflection()
        {
            var result = new ParallelLoopResult();

            var type = typeof(ParallelLoopResult);
            var completed_field = type.GetField("m_completed", BindingFlags.Instance | BindingFlags.NonPublic);
            completed_field.SetValue(result, true);

            return result;
        }
    }
}
