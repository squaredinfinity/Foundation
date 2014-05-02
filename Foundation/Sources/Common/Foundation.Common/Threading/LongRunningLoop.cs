using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public class LongRunningLoop : ILongRunningLoop
    {
        CancellationTokenSource OperationCancellationTokenSource;

        Task OperationTask;

        ReaderWriterLockSlimEx UpdateLock = new ReaderWriterLockSlimEx();

        Action LoopBody { get; set; }

        TimeSpan _loopIterationInterval = TimeSpan.Zero;
        public TimeSpan LoopIterationInterval
        {
            get { return _loopIterationInterval; }
            set
            {
                if (_loopIterationInterval == value)
                    return;

                _loopIterationInterval = value;

                ReconstructOperationTask();
            }
        }

        protected void OperationLoop(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                try
                {
                    LoopBody();

                    Thread.Sleep(LoopIterationInterval);
                }
                catch (Exception ex)
                {
                    // notify about error via event
                }
            }
        }

        public LongRunningLoop(Action loopBody)
            : this(loopBody, Timeout.InfiniteTimeSpan)
        { }

        public LongRunningLoop(Action actionToExecute, TimeSpan loopIterationInterval)
        {
            LoopBody = actionToExecute;

            LoopIterationInterval = loopIterationInterval;
        }

        void ReconstructOperationTask()
        {
            using (UpdateLock.AcquireWriteLock())
            {
                if (OperationCancellationTokenSource != null)
                {
                    OperationCancellationTokenSource.Cancel();
                    OperationTask.Wait(250, OperationCancellationTokenSource.Token);

                    OperationCancellationTokenSource = null;
                    OperationTask = null;
                }

                if (LoopIterationInterval == Timeout.InfiniteTimeSpan)
                    return;

                OperationCancellationTokenSource = new CancellationTokenSource();

                OperationTask =
                    Task.Factory.StartNew(
                    () => OperationLoop(OperationCancellationTokenSource.Token),
                    OperationCancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }
        }

        public void Cancel()
        {
            LoopIterationInterval = Timeout.InfiniteTimeSpan;
        }
    }
}
