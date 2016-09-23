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
        bool IsCompleted { get; }
        void Wait(bool ignoreCanceledExceptions);
        bool Wait(TimeSpan timeout, bool ignoreCanceledExceptions);
        void Cancel();
    }

    public class AsyncOperationRequest : IAsyncOperationRequest
    {
        Task Task;
        CancellationTokenSource CancellationTokenSource;

        public bool IsCompleted
        {
            get { return Task.IsCompleted; }
        }

        public AsyncOperationRequest(Task operationTask, CancellationTokenSource cancellationTokenSource)
        {
            this.Task = operationTask;

            this.CancellationTokenSource = cancellationTokenSource;
        }

        public void Start()
        {
            Task.Start();
        }

        public void Wait(bool ignoreCanceledExceptions)
        {
            Task.Wait(CancellationTokenSource.Token, ignoreCanceledExceptions);
        }

        public bool Wait(TimeSpan timeout, bool ignoreCanceledExceptions)
        {
            return Task.Wait((int)timeout.TotalMilliseconds, CancellationTokenSource.Token, ignoreCanceledExceptions);
        }

        public void Cancel()
        {
            CancellationTokenSource.Cancel();
        }
    }

    public class AsyncAction : IAsyncAction
    {
        ILock Lock = new ReaderWriterLockSlimEx();

        IAsyncOperationRequest CurrentOperation { get; set; }
        IAsyncOperationRequest NextOperation { get; set; }

        Action ActionToExecute { get; set; }
        Action<CancellationToken> CancellableActionToExecute { get; set; }
        
        public IAsyncOperationRequest RequestExecute()
        {
            return RequestExecute(beforeExecute: null, afterExecute: null);
        }
        
        public IAsyncOperationRequest RequestExecute(Action<CancellationToken> beforeExecute, Action<CancellationToken> afterExecute)
        {
            using (Lock.AcquireWriteLock())
            {
                CancelCurrentRequest();

                var cts = new CancellationTokenSource();

                var task = (Task)null;


                //# Before Execute

                if (beforeExecute != null)
                {
                    task =
                        new Task(() => beforeExecute(cts.Token), cts.Token);

                    // ContinueWith() returns a continuation task, which may be ignored
                    task.ContinueWith(_prevTask => task, cts.Token);
                }
                else
                {
                    task = new Task(() => { }, cts.Token);
                }

                //# Execute

                if (CancellableActionToExecute != null)
                {
                    task =
                        new Task(() =>
                            CancellableActionToExecute(cts.Token),
                            cts.Token);
                }

                if (CancellableActionToExecute != null)
                {
                    if (task == null)
                    {
                        task =
                           new Task(() =>
                               CancellableActionToExecute(cts.Token),
                               cts.Token);
                    }
                    else
                    {
                        task.ContinueWith(_prevTask => CancellableActionToExecute(cts.Token), cts.Token);
                    }
                }
                else
                {
                    if (task == null)
                    {
                        task =
                            new Task(() =>
                                ActionToExecute(),
                                cts.Token);
                    }
                    else
                    {
                        task.ContinueWith(_prevTask => 
                            {
                                if(!cts.IsCancellationRequested)
                                    ActionToExecute();

                            }, cts.Token);
                    }
                }

                //# After Execute

                if(afterExecute != null)
                {
                    task.ContinueWith(_prevTask => afterExecute(cts.Token), cts.Token);
                }

                var op = new AsyncOperationRequest(task, cts);

                op.Start();

                CurrentOperation = op;
            }

            return CurrentOperation;
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
            if (CurrentOperation != null)
            {
                CurrentOperation.Cancel();
                CurrentOperation.Wait(ignoreCanceledExceptions: true);
            }
        }
    }

    public class AsyncAction<T> : IAsyncAction<T>
    {
        ILock Lock = new ReaderWriterLockSlimEx();

        IAsyncOperationRequest CurrentOperation { get; set; }
        IAsyncOperationRequest NextOperation { get; set; }

        Action<T> ActionToExecute { get; set; }
        Action<T, CancellationToken> CancellableActionToExecute { get; set; }
                
        public IAsyncOperationRequest RequestExecute(T argument)
        {
            using (Lock.AcquireWriteLock())
            {
                CancelCurrentRequest();

                var cts = new CancellationTokenSource();

                var task = (Task)null;

                if (CancellableActionToExecute != null)
                {
                    task =
                        new Task(() =>
                            CancellableActionToExecute(argument, cts.Token),
                            cts.Token);
                }
                else
                {
                    task =
                        new Task(() =>
                            ActionToExecute(argument),
                            cts.Token);
                }

                var op = new AsyncOperationRequest(task, cts);

                op.Start();

                CurrentOperation = op;
            }

            return CurrentOperation;
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
            if (CurrentOperation != null)
            {
                CurrentOperation.Cancel();
                CurrentOperation.Wait(ignoreCanceledExceptions: true);
            }
        }
    }
}
