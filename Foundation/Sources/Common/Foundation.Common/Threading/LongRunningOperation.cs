using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public class LongRunningOperation
    {
        CancellationTokenSource OperationCancellationTokenSource;

        Task OperationTask;

        AutoResetEvent ExecuteRequestedEvent = new AutoResetEvent(false);

        public TimeSpan Throttle { get; set; }

        Action ActionToExecute { get; set; }

        void OperationLoop(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                if (!ExecuteRequestedEvent.WaitOne(millisecondsTimeout: 250))
                    continue;

                try
                {
                    ActionToExecute();
                }
                catch (Exception ex)
                {
                    // notify about error via event
                }
            }
        }

        public void RequestExecute()
            {
                ExecuteRequestedEvent.Set();
            }

        public LongRunningOperation(Action actionToExecute)
            {
                this.ActionToExecute = actionToExecute;

                OperationCancellationTokenSource = new CancellationTokenSource();

                OperationTask =
                    Task.Factory.StartNew(
                    () => OperationLoop(OperationCancellationTokenSource.Token),
                    OperationCancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }
    }
}
