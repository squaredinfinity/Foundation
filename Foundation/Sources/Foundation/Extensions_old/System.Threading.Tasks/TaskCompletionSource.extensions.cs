using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class TaskCompletionSourceExtensions
    {
        /// <summary>
        /// Sets result ofTaskCompletionSource from specified task.
        /// It will propagate faulted and cancelled states to TaskCompletionSource.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tcs"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public static bool TrySetResult<TResult>(this TaskCompletionSource<TResult> tcs, Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            switch (task.Status)
            {
                case TaskStatus.Faulted:
                    return tcs.TrySetException(task.Exception);
                case TaskStatus.Canceled:
                    return tcs.TrySetCanceled();
                case TaskStatus.RanToCompletion:
                    var t = task as Task<TResult>;
                    if (t == null)
                    {
                        return tcs.TrySetResult(default(TResult));
                    }
                    else
                    {
                        return tcs.TrySetResult(t.Result);
                    }
                default:
                    throw new NotSupportedException($"{task.Status} is not supported");
            }
        }

        public static void TimeoutAfter<TResult>(this TaskCompletionSource<TResult> tcs, TimeSpan timeout)
        {
            tcs.TimeoutAfter((int)timeout.TotalMilliseconds, (_tcs) => _tcs.SetException(new TimeoutException()));
        }

        public static void TimeoutAfter<TResult>(this TaskCompletionSource<TResult> tcs, int millisecondsTimeout)
        {
            tcs.TimeoutAfter(millisecondsTimeout, (_tcs) => _tcs.SetException(new TimeoutException()));
        }

        public static void TimeoutAfter<TResult>(this TaskCompletionSource<TResult> tcs, TimeSpan timeout, Action<TaskCompletionSource<TResult>> onTimeout)
        {
            tcs.TimeoutAfter((int)timeout.TotalMilliseconds, onTimeout);
        }
        public static void TimeoutAfter<TResult>(this TaskCompletionSource<TResult> tcs, int millisecondsTimeout, Action<TaskCompletionSource<TResult>> onTimeout)
        {
            if (millisecondsTimeout == Timeout.Infinite)
                return;

            // timed out already
            if (millisecondsTimeout == 0)
            {
                onTimeout(tcs);
            }

            // set up timeout behavior
            var delay_cts = new CancellationTokenSource();
            Task.Delay(millisecondsTimeout, delay_cts.Token)
                .ContinueWith((_previous_task, _state) =>
                {
                    if (_previous_task.Status == TaskStatus.RanToCompletion)
                    {
                        // timeout occured
                        var _tcs = _state as TaskCompletionSource<TResult>;
                        onTimeout(_tcs);
                    }
                }, tcs, delay_cts.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);

            // set up taks completion behavior
            tcs.Task.ContinueWith((_previous_task, _state) =>
            {
                var cts = (CancellationTokenSource)_state;
                // cancel delay task
                cts.Cancel();
            },
            delay_cts,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        }
    }
}
