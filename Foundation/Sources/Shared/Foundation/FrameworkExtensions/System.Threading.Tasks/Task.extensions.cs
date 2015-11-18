using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class TaskExtensions
    {
        //
        // Summary:
        //     Waits for the System.Threading.Tasks.Task to complete execution.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The System.Threading.Tasks.Task has been disposed.
        //
        //   System.AggregateException:
        //     The System.Threading.Tasks.Task was canceled -or- an exception was thrown
        //     during the execution of the System.Threading.Tasks.Task. If the task was
        //     canceled, the System.AggregateException contains an System.OperationCanceledException
        //     in its System.AggregateException.InnerExceptions collection.
        public static void Wait(this Task task, bool ignoreCanceledExceptions)
        {
            if (ignoreCanceledExceptions)
            {
                try
                {
                    task.Wait();
                }
                catch (TaskCanceledException)
                { /* expected */ }
                catch (OperationCanceledException)
                { /* expected */ }
            }
            else
            {
                task.Wait();
            }
        }

        //
        // Summary:
        //     Waits for the System.Threading.Tasks.Task to complete execution.
        //
        // Parameters:
        //   cancellationToken:
        //     A System.Threading.Tasks.TaskFactory.CancellationToken to observe while waiting
        //     for the task to complete.
        //
        // Exceptions:
        //   System.OperationCanceledException:
        //     The cancellationToken was canceled.
        //
        //   System.ObjectDisposedException:
        //     The System.Threading.Tasks.Task has been disposed.
        //
        //   System.AggregateException:
        //     The System.Threading.Tasks.Task was canceled -or- an exception was thrown
        //     during the execution of the System.Threading.Tasks.Task. If the task was
        //     canceled, the System.AggregateException contains an System.OperationCanceledException
        //     in its System.AggregateException.InnerExceptions collection.
        public static void Wait(this Task task, CancellationToken cancellationToken, bool ignoreCanceledExceptions)
        {
            if (ignoreCanceledExceptions)
            {
                try
                {
                    task.Wait(cancellationToken);
               }
                catch (TaskCanceledException)
                { /* expected */ }
                catch (OperationCanceledException)
                { /* expected */ }
            }
            else
            {
                task.Wait();
            }
        }

        //
        // Summary:
        //     Waits for the System.Threading.Tasks.Task to complete execution.
        //
        // Parameters:
        //   millisecondsTimeout:
        //     The number of milliseconds to wait, or System.Threading.Timeout.Infinite
        //     (-1) to wait indefinitely.
        //
        // Returns:
        //     true if the System.Threading.Tasks.Task completed execution within the allotted
        //     time; otherwise, false.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The System.Threading.Tasks.Task has been disposed.
        //
        //   System.ArgumentOutOfRangeException:
        //     millisecondsTimeout is a negative number other than -1, which represents
        //     an infinite time-out.
        //
        //   System.AggregateException:
        //     The System.Threading.Tasks.Task was canceled -or- an exception was thrown
        //     during the execution of the System.Threading.Tasks.Task. If the task was
        //     canceled, the System.AggregateException contains an System.OperationCanceledException
        //     in its System.AggregateException.InnerExceptions collection.
        public static bool Wait(this Task task, int millisecondsTimeout, bool ignoreCanceledExceptions)
        {
            if (ignoreCanceledExceptions)
            {
                try
                {
                    return task.Wait(millisecondsTimeout);
                }
                catch (TaskCanceledException)
                { return false; }
                catch (OperationCanceledException)
                { return false; }
            }
            else
            {
                return task.Wait(millisecondsTimeout);
            }
        }

        //
        // Summary:
        //     Waits for the System.Threading.Tasks.Task to complete execution.
        //
        // Parameters:
        //   timeout:
        //     A System.TimeSpan that represents the number of milliseconds to wait, or
        //     a System.TimeSpan that represents -1 milliseconds to wait indefinitely.
        //
        // Returns:
        //     true if the System.Threading.Tasks.Task completed execution within the allotted
        //     time; otherwise, false.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The System.Threading.Tasks.Task has been disposed.
        //
        //   System.ArgumentOutOfRangeException:
        //     timeout is a negative number other than -1 milliseconds, which represents
        //     an infinite time-out -or- timeout is greater than System.Int32.MaxValue.
        //
        //   System.AggregateException:
        //     The System.Threading.Tasks.Task was canceled -or- an exception was thrown
        //     during the execution of the System.Threading.Tasks.Task.
        public static bool Wait(this Task task, TimeSpan timeout, bool ignoreCanceledExceptions)
        {
            if (ignoreCanceledExceptions)
            {
                try
                {
                    return task.Wait(timeout);
                }
                catch (TaskCanceledException)
                { return false; }
                catch (OperationCanceledException)
                { return false; }
            }
            else
            {
                return task.Wait(timeout);
            }
        }

        //
        // Summary:
        //     Waits for the System.Threading.Tasks.Task to complete execution.
        //
        // Parameters:
        //   millisecondsTimeout:
        //     The number of milliseconds to wait, or System.Threading.Timeout.Infinite
        //     (-1) to wait indefinitely.
        //
        //   cancellationToken:
        //     A System.Threading.Tasks.TaskFactory.CancellationToken to observe while waiting
        //     for the task to complete.
        //
        // Returns:
        //     true if the System.Threading.Tasks.Task completed execution within the allotted
        //     time; otherwise, false.
        //
        // Exceptions:
        //   System.OperationCanceledException:
        //     The cancellationToken was canceled.
        //
        //   System.ObjectDisposedException:
        //     The System.Threading.Tasks.Task has been disposed.
        //
        //   System.ArgumentOutOfRangeException:
        //     millisecondsTimeout is a negative number other than -1, which represents
        //     an infinite time-out.
        //
        //   System.AggregateException:
        //     The System.Threading.Tasks.Task was canceled -or- an exception was thrown
        //     during the execution of the System.Threading.Tasks.Task. If the task was
        //     canceled, the System.AggregateException contains an System.OperationCanceledException
        //     in its System.AggregateException.InnerExceptions collection.
        public static bool Wait(this Task task, int millisecondsTimeout, CancellationToken cancellationToken, bool ignoreCanceledExceptions)
        {
            if (ignoreCanceledExceptions)
            {
                try
                {
                    return task.Wait(millisecondsTimeout, cancellationToken);
                }
                catch (TaskCanceledException)
                { return false; }
                catch (OperationCanceledException)
                { return false; }
            }
            else
            {
                return task.Wait(millisecondsTimeout, cancellationToken);
            }
        }
    }
}
