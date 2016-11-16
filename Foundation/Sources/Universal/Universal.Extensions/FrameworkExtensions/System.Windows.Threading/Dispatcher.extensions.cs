using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class UIDispatcherExtensions
    {
        public static TaskScheduler GetUIDispatcherScheduler()
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;

            return dispatcher.GetTaskSchedulerAsync().Result;
        }
    }
}
    