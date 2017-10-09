using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class TaskExtensions
    {
        #region Try Wait

        /// <summary>
        /// Waits for the System.Threading.Tasks.Task to complete execution.
        /// </summary>
        /// <param name="task"></param>
        /// <returns>true if task run to completion, false if task was cancelled or timedout</returns>
        public static bool TryWait(this Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is TaskCanceledException)
                    return false;
                if (ex.InnerException is OperationCanceledException)
                    return false;
                if (ex.InnerException is TimeoutException)
                    return false;

                throw;
            }
            catch (TaskCanceledException)
            { return false; }
            catch (OperationCanceledException)
            { return false; }
            catch (TimeoutException)
            { return false; }

            return true;
        }

        /// <summary>
        /// Waits for the System.Threading.Tasks.Task to complete execution.
        /// </summary>
        /// <param name="task"></param>
        /// <returns>true if task run to completion, false if task was cancelled or timedout</returns>
        public static bool TryWait(this Task task, CancellationToken cancellationToken)
        {
            try
            {
                task.Wait(cancellationToken);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is TaskCanceledException)
                    return false;
                if (ex.InnerException is OperationCanceledException)
                    return false;
                if (ex.InnerException is TimeoutException)
                    return false;

                throw;
            }
            catch (TaskCanceledException)
            { return false; }
            catch (OperationCanceledException)
            { return false; }
            catch (TimeoutException)
            { return false; }

            return true;
        }

        /// <summary>
        /// Waits for the System.Threading.Tasks.Task to complete execution.
        /// </summary>
        /// <param name="task"></param>
        /// <returns>true if task run to completion, false if task was cancelled or timed out</returns>
        public static bool TryWait(this Task task, int millisecondsTimeout)
        {
            try
            {
                return task.Wait(millisecondsTimeout);
            }
            catch(AggregateException ex)
            {
                if (ex.InnerException is TaskCanceledException)
                    return false;
                if (ex.InnerException is OperationCanceledException)
                    return false;
                if (ex.InnerException is TimeoutException)
                    return false;

                throw;
            }
            catch (TaskCanceledException)
            { return false; }
            catch (OperationCanceledException)
            { return false; }
            catch (TimeoutException)
            { return false; }
        }

        /// <summary>
        /// Waits for the System.Threading.Tasks.Task to complete execution.
        /// </summary>
        /// <param name="task"></param>
        /// <returns>true if task run to completion, false if task was cancelled or timedout</returns>
        public static bool TryWait(this Task task, TimeSpan timeout)
        {

            try
            {
                return task.Wait(timeout);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is TaskCanceledException)
                    return false;
                if (ex.InnerException is OperationCanceledException)
                    return false;
                if (ex.InnerException is TimeoutException)
                    return false;

                throw;
            }
            catch (TaskCanceledException)
            { return false; }
            catch (OperationCanceledException)
            { return false; }
            catch (TimeoutException)
            { return false; }
        }

        /// <summary>
        /// Waits for the System.Threading.Tasks.Task to complete execution.
        /// </summary>
        /// <param name="task"></param>
        /// <returns>true if task run to completion, false if task was cancelled or timedout</returns>
        public static bool TryWait(this Task task, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            try
            {
                return task.Wait(millisecondsTimeout, cancellationToken);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is TaskCanceledException)
                    return false;
                if (ex.InnerException is OperationCanceledException)
                    return false;
                if (ex.InnerException is TimeoutException)
                    return false;

                throw;
            }
            catch (TaskCanceledException)
            { return false; }
            catch (OperationCanceledException)
            { return false; }
            catch (TimeoutException)
            { return false; }
        }

        #endregion

        #region Timeout After

        
        // this is not yet ready for prime time
        static Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
            if (task.IsCompleted || (millisecondsTimeout == Timeout.Infinite))
                return task;

            // int becasue there's no non-generic TaskCompletionSource so have to use something
            var tcs = new TaskCompletionSource<int>();

            // timed out alread
            if (millisecondsTimeout == 0)
            {
                tcs.SetException(new TimeoutException());
                return tcs.Task;
            }

            var delay_cts = new CancellationTokenSource();

            Task.Delay(millisecondsTimeout, delay_cts.Token)
                .ContinueWith((_previous_task, _state) =>
                {
                    if (_previous_task.Status == TaskStatus.RanToCompletion)
                    {
                        // timeout occured
                        var _tcs = _state as TaskCompletionSource<int>;
                        _tcs.TrySetException(new TimeoutException());
                    }
                }, tcs, delay_cts.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);

            task.ContinueWith((_previous_task, _state) =>
            {
                var t = (Tuple<CancellationTokenSource, TaskCompletionSource<int>>)_state;
                // cancel delay task
                t.Item1.Cancel();
                // update TaskCompletionSource state from the task
                t.Item2.TrySetResult<int>(_previous_task);
            },
            Tuple.Create(delay_cts, tcs),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

            return tcs.Task;
        }

        #endregion
    }
}
