using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Threading
{
    public class AsyncLoop : IAsyncLoop
    {
        //CancellationTokenSource CancellationTokenSource;
        Task OperationTask;

        WeakDelegate<Action> LoopBodyReference { get; set; }
        
        public AsyncLoop(Action loopBody)
        {
            LoopBodyReference = new WeakDelegate<Action>(this, loopBody);
        }
        
        public void Start(TimeSpan loopIterationDelay, CancellationToken cancellationToken)
        {
            var _loopBody = LoopBodyReference;
            var _loopIterationDelay = loopIterationDelay;
            var _cancellationToken = cancellationToken;

            OperationTask = 
                Task.Factory.StartNew(
                () => OperationLoop(LoopBodyReference, _loopIterationDelay, _cancellationToken), 
                cancellationToken, 
                TaskCreationOptions.LongRunning, 
                TaskScheduler.Default);
        }

        static void OperationLoop(WeakDelegate<Action> loopBodyReference, TimeSpan loopIterationDelay, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                try
                {
                    if (!loopBodyReference.TryExecute())
                        return;

                    Task.Delay(loopIterationDelay, cancellationToken).Wait();
                }
                catch (ThreadAbortException)
                { }
                catch (Exception ex)
                {
                    // notify about error via event
                }
            }
        }

        ~AsyncLoop()
        {

        }
    }
}
