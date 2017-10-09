using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class TaskFactoryExtensions
    {
        #region Start New Thread

        /// <summary>
        /// Creates a task to execute on a different thread.
        /// </summary>
        /// <param name="taskFactory"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task StartNewThread(this TaskFactory taskFactory, Action action)
        {
            await taskFactory.StartNew(action, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Creates a task to execute on a different thread.
        /// </summary>
        /// <param name="taskFactory"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task StartNewThread(this TaskFactory taskFactory, Action action, CancellationToken ct)
        {
            await taskFactory.StartNew(action, ct, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Creates a task to execute on a different thread.
        /// </summary>
        /// <param name="taskFactory"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task<TResult> StartNewThread<TResult>(this TaskFactory taskFactory, Func<TResult> func)
        {
            return await taskFactory.StartNew(func, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Creates a task to execute on a different thread.
        /// </summary>
        /// <param name="taskFactory"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task<TResult> StartNewThread<TResult>(this TaskFactory taskFactory, Func<TResult> func, CancellationToken ct)
        {
            return await taskFactory.StartNew(func, ct, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        #endregion
    }
}
