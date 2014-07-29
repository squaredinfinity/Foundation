using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Threading
{
    public interface IAsyncOperationRequest
    {
        void Wait();
    }

    public class AsyncOperationRequest : IAsyncOperationRequest
    {
        Task Task;
        CancellationToken CancellationToken;

        public AsyncOperationRequest(Task operationTask, CancellationToken cancellationToken)
        {
            this.Task = operationTask;
            this.CancellationToken = cancellationToken;
        }

        public void Wait()
        {
            Task.Wait(CancellationToken);
        }
    }

    public class AsyncAction : IAsyncAction
    {
        CancellationTokenSource OperationCancellationTokenSource;

        Task OperationTask;
        
        public TimeSpan RequestsThrottle { get; set; }

        Action ActionToExecute { get; set; }        
        Action<CancellationToken> CancellableActionToExecute { get; set; }
        
        public IAsyncOperationRequest RequestExecute()
        {
            CancelCurrentRequest();

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

        public void Dispose()
        {
            CancelCurrentRequest();
        }

        void CancelCurrentRequest()
        {
            if (OperationTask != null)
            {
                OperationCancellationTokenSource.Cancel();

                try
                {
                    OperationTask.Wait(OperationCancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // expected
                }
            }
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
            CancelCurrentRequest();

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

        public void Dispose()
        {
            CancelCurrentRequest();
        }

        void CancelCurrentRequest()
        {
            if (OperationTask != null)
            {
                OperationCancellationTokenSource.Cancel();
                OperationTask.Wait(OperationCancellationTokenSource.Token, ignoreCanceledExceptions: true);
            }
        }
    }
}
