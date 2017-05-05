using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SquaredInfinity.Extensions
{
    public static class DispatcherExtensions
    {
        public static TaskScheduler GetUIDispatcherScheduler()
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;

            return dispatcher.GetTaskSchedulerAsync().Result;
        }

        public static TaskScheduler GetTaskScheduler(
            this Dispatcher dispatcher,
            DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var tcs = new TaskCompletionSource<TaskScheduler>();

            if (dispatcher.Thread.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
            {
                return TaskScheduler.FromCurrentSynchronizationContext();
            }

            return dispatcher.Invoke(() => TaskScheduler.FromCurrentSynchronizationContext(), priority);
        }

        public static Task<TaskScheduler> GetTaskSchedulerAsync(
            this Dispatcher dispatcher,
            DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var tcs = new TaskCompletionSource<TaskScheduler>();

            if (dispatcher.Thread.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
            {
                tcs.SetResult(TaskScheduler.FromCurrentSynchronizationContext());
                return tcs.Task;
            }

            return dispatcher.InvokeAsync<TaskScheduler>(() => TaskScheduler.FromCurrentSynchronizationContext(), priority).Task;
        }
    }
}
    