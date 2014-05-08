using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public class AsyncAction : IAsyncAction
    {
        CancellationTokenSource OperationCancellationTokenSource;

        Task OperationTask;
        
        public TimeSpan RequestsThrottle { get; set; }

        Action ActionToExecute { get; set; }
        
        Action<CancellationToken> CancellableActionToExecute { get; set; }
        
        public void RequestExecute()
        {
            if(OperationTask != null)
            {
                OperationCancellationTokenSource.Cancel();
                OperationTask.Wait();
            }

            OperationCancellationTokenSource = new CancellationTokenSource();

            if (CancellableActionToExecute != null)
            {
                OperationTask =
                    new Task(() => 
                        CancellableActionToExecute(OperationCancellationTokenSource.Token),
                        OperationCancellationTokenSource.Token);
            }
            else
            {
                OperationTask =
                    new Task(() =>
                        ActionToExecute(),
                        OperationCancellationTokenSource.Token);
            }

            OperationTask.Start();
        }

        public AsyncAction(Action actionToExecute)
        {
            this.ActionToExecute = actionToExecute;
        }

        public AsyncAction(Action<CancellationToken> cancellableActionToExecute)
        {
            this.CancellableActionToExecute = cancellableActionToExecute;
        }
    }

    public class AsyncAction<T> : IAsyncAction<T>
    {
        CancellationTokenSource OperationCancellationTokenSource;

        Task OperationTask;

        public TimeSpan RequestsThrottle { get; set; }

        Action<T> ActionToExecute { get; set; }

        Action<T, CancellationToken> CancellableActionToExecute { get; set; }

        public void RequestExecute(T argument)
        {
            if (OperationTask != null)
            {
                OperationCancellationTokenSource.Cancel();
                OperationTask.Wait();
            }

            OperationCancellationTokenSource = new CancellationTokenSource();

            if (CancellableActionToExecute != null)
            {
                OperationTask =
                    new Task(() =>
                        CancellableActionToExecute(argument, OperationCancellationTokenSource.Token),
                        OperationCancellationTokenSource.Token);
            }
            else
            {
                OperationTask =
                    new Task(() =>
                        ActionToExecute(argument),
                        OperationCancellationTokenSource.Token);
            }

            OperationTask.Start();
        }

        public AsyncAction(Action<T> actionToExecute)
        {
            this.ActionToExecute = actionToExecute;
        }

        public AsyncAction(Action<T, CancellationToken> cancellableActionToExecute)
        {
            this.CancellableActionToExecute = cancellableActionToExecute;
        }
    }
}
