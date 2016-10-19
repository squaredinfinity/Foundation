using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation
{
    [DebuggerDisplay("WorkItem {Id}")]
    public class InvocationThrottleWorkItem
    {
        public Guid Id { get; } = Guid.NewGuid();

        public DateTime? CompleteTimeUtc { get; internal set; }
        public DateTime RequestWindowTimeUTC { get; internal set; }
        public DateTime RequestTimeUTC { get; internal set; }
        public object State { get; internal set; }
        public Task Task { get; internal set; }
        public bool MustRunToCompletion { get; internal set; }
        public CancellationTokenSource CancellationTokenSource { get; internal set; }
        internal Action<object, InvocationThrottleWorkItem, CancellationToken> Action { get; set; }
    }

    public class InvocationThrottle
    {
        readonly object Sync = new object();

        public TimeSpan Min { get; private set; } = TimeSpan.FromMilliseconds(50);
        public TimeSpan? Max { get; private set; } = null;

        public TaskScheduler Scheduler { get; private set; } = TaskScheduler.Default;
        
        public InvocationThrottleWorkItem CurrentWorkItem { get; private set; } = null;
        public InvocationThrottleWorkItem PendingWorkItem { get; private set; } = null;

        readonly System.Timers.Timer MinTimer;
        DateTime LastMinTimerResetUTC;

        public InvocationThrottle()
            : this(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(250))
        {}

        public InvocationThrottle(TimeSpan min)
            : this(min, null)
        { }

        public InvocationThrottle(TimeSpan min, TimeSpan? max)
        {
            MinTimer = new System.Timers.Timer();
            Min = min;
            Max = max;

            MinTimer.Interval = Min.TotalMilliseconds;

            MinTimer.Elapsed += MinTimer_Elapsed;
        }

        public void Update(TimeSpan min, TimeSpan? max = null)
        {
            lock (Sync)
            {
                Min = min;
                Max = max;                
                MinTimer.Interval = min.TotalMilliseconds;
            }
        }

        void MinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock(Sync)
            {
                MinTimer.Stop();

                if (LastMinTimerResetUTC >= e.SignalTime.ToUniversalTime())
                {
                    // Timer Elapsed, but got canceled before it could stop this event from occuring;
                    return;
                }

                OnNext();
            }
        }

        void OnNext()
        {
            lock (Sync)
            {
                if (PendingWorkItem == null)
                    return;

                var old_current = CurrentWorkItem;

                CurrentWorkItem = PendingWorkItem;
                PendingWorkItem = null;

                var wi = CurrentWorkItem;

                wi.CancellationTokenSource = new CancellationTokenSource();

                var ct = wi.CancellationTokenSource.Token;

                wi.Task = Task.Factory.StartNew(() =>
                {
                    if (old_current != null)
                    {
                        old_current.Task.Wait(ignoreCanceledExceptions: true);
                    }

                    if (ct.IsCancellationRequested)
                        return;

                    wi.Action(wi.State, wi, ct);

                    lock (Sync)
                    {
                        if (ct.IsCancellationRequested)
                            return;

                        var utc_now = DateTime.UtcNow;

                        // action completed succesfully

                        CurrentWorkItem = null;
                        wi.CompleteTimeUtc = DateTime.UtcNow;

                        // check if there is a task pending
                        if (PendingWorkItem != null)
                        {
                            // it is, but within min,
                            // reset timer to start from now
                            if (utc_now - PendingWorkItem.RequestWindowTimeUTC < Min)
                            {
                                // nothing to do here, timer will take care of it
                                return;
                            }
                            else
                            {
                                // pending is waiting > min
                                // invoke pending action immediately, since its been waiting long enough
                                OnNext();
                                return;
                            }
                        }
                        else
                        {
                            // no work items pending
                            // nothing else to do here
                        }
                    }

                }, ct, TaskCreationOptions.None, Scheduler);
            }
        }

        public void InvokeAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            InvokeAsync((state, wi, ct) => action(), null);
            
        }

        public void InvokeAsync(Action<CancellationToken> cancellableAction)
        {
            if (cancellableAction == null)
                throw new ArgumentNullException(nameof(cancellableAction));

            InvokeAsync((state, wi, ct) => cancellableAction(ct), null);
        }

        public void InvokeAsync(Action<object, CancellationToken> cancellableActionWithState)
        {
            if (cancellableActionWithState == null)
                throw new ArgumentNullException(nameof(cancellableActionWithState));

            InvokeAsync((state, wi, ct) => cancellableActionWithState(state, ct), null);
        }

        public void InvokeAsync(Action<object, InvocationThrottleWorkItem, CancellationToken> cancellableActionWithState, object state)
        {
            if (cancellableActionWithState == null)
                throw new ArgumentNullException(nameof(cancellableActionWithState));

            lock (Sync)
            {
                var wi = new InvocationThrottleWorkItem();
                wi.RequestTimeUTC = DateTime.UtcNow;
                wi.State = state;
                wi.Action = cancellableActionWithState;

                // Pending request alredy exists
                if (PendingWorkItem != null)
                {
                    // within min distance of last pending request
                    if (DateTime.UtcNow - PendingWorkItem.RequestTimeUTC < Min)
                    {
                        // update window request time (to be able to check for max later)
                        wi.RequestWindowTimeUTC = PendingWorkItem.RequestWindowTimeUTC;
                        PendingWorkItem = wi;

                        // past Max waiting period, invoke
                        if (Max != null && DateTime.UtcNow - wi.RequestWindowTimeUTC >= Max)
                        {
                            // past Max waiting time, invoke and ensure runs to completion
                            wi.MustRunToCompletion = true;
                            OnNext();
                            return;
                        }

                        ResetMinTimer();

                        return;
                    }
                    else // passed min distance, invoke
                    {
                        PendingWorkItem = wi;
                        OnNext();
                        return;
                    }
                }
                else // there's no pending request
                {
                    wi.RequestWindowTimeUTC = wi.RequestTimeUTC;
                    PendingWorkItem = wi;

                    if (CurrentWorkItem == null)
                    {
                        // wait Min before executing
                        ResetMinTimer();

                        return;
                    }
                    else
                    {
                        if (CurrentWorkItem.MustRunToCompletion)
                        {
                            // continuation of Current work item will handle further processing
                        }
                        else
                        {
                            // cancel current
                            CurrentWorkItem.CancellationTokenSource.Cancel();
                            CurrentWorkItem.Task.ContinueWith((_) => OnNext(), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously);
                        }
                    }
                }
            }
        }

        void ResetMinTimer()
        {
            MinTimer.Stop();
            LastMinTimerResetUTC = DateTime.UtcNow;
            MinTimer.Start();
        }
    }

    public partial class InvocationThrottle_OLD
    {
        class ActionInfo
        {
            readonly object Sync = new object();

            Action<CancellationToken> CancellableAction = null;
            Action Action = null;

            WorkItem RunningWorkItem = null;

            public TimeSpan Timeout { get; internal set; }

            public DateTime LastCompleteTimeUtc { get; internal set; } = DateTime.MinValue;

            public ActionInfo()
            { }

            class WorkItem
            {
                public Action Action;
                public Action<CancellationToken> CancellableAction;
                public CancellationTokenSource CancellationTokenSource;

                public Task PreviousActionTask { get; internal set; }
                public bool ShouldContinueWithNextAction { get; internal set; } = true;
                public bool ShouldRunToCompletion { get; internal set; }
                public Task Task { get; internal set; }
                public Task ContinuationTask { get; internal set; }
                public TimeSpan Timeout { get; internal set; }
            }

            public bool TryInvokeAsync(bool should_run_to_completion)
            {
                lock (Sync)
                {
                    // there's no pending action or cancellable action
                    // just return
                    if (Action == null && CancellableAction == null)
                    {
                        return false;
                    }

                    // last task is set, but cannot be cancelled
                    // when it's finished it should continue with next action (if one has already been set)
                    if (RunningWorkItem != null && RunningWorkItem.CancellableAction == null)
                    {
                        RunningWorkItem.ShouldContinueWithNextAction = true;
                        return false;
                    }

                    // if last task should run to completion, don't cancel it
                    // new action will be processed after last completes
                    if(RunningWorkItem != null && RunningWorkItem.ShouldRunToCompletion)
                    {
                        RunningWorkItem.ShouldContinueWithNextAction = true;
                        return false;
                    }

                    // last task doesn't have to finish, and can be cancelled
                    // cancel it now
                    if (RunningWorkItem != null && RunningWorkItem.CancellationTokenSource != null)
                    {
                        if (!RunningWorkItem.ShouldRunToCompletion)
                        {
                            RunningWorkItem.CancellationTokenSource.Cancel();
                            RunningWorkItem = null;
                        }
                    }

                    var payload = new WorkItem
                    {
                        Action = Action,
                        Timeout = Timeout,
                        CancellableAction = CancellableAction,
                        CancellationTokenSource = new CancellationTokenSource(Timeout),
                        ShouldRunToCompletion = should_run_to_completion
                    };

                    if (RunningWorkItem != null && RunningWorkItem.ShouldRunToCompletion)
                    {
                        payload.PreviousActionTask = RunningWorkItem.Task;
                    }

                    var new_task = new Task(DoAction, payload, payload.CancellationTokenSource.Token);
                    

                    if (RunningWorkItem != null)
                    {
                        payload.Task = 
                            RunningWorkItem
                            .Task
                            .ContinueWith(
                                _prev => new_task, 
                                payload.CancellationTokenSource.Token,
                                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.LazyCancellation, 
                                TaskScheduler.Default);
                    }
                    else
                    {
                        payload.Task = new_task;
                    }

                    // call self after finished to invoke any pending actions
                    payload.Task =
                        payload.Task
                        .ContinueWith(
                            _prev => OnLastTaskFinished(payload),
                            TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.LazyCancellation);

                    // start task if there's no previous action
                    // othwerwise task will be a continueation of previous
                    if(RunningWorkItem == null)
                        new_task.Start();

                    RunningWorkItem = payload;

                    Action = null;
                    CancellableAction = null;

                    return true;
                }
            }

            static void DoAction(object payloadAsObject)
            {
                var payload = (WorkItem)payloadAsObject;

                if (payload.PreviousActionTask != null)
                    payload.PreviousActionTask.Wait((int)payload.Timeout.TotalMilliseconds, payload.CancellationTokenSource.Token);

                if (payload.Action != null)
                {
                    payload.Action();
                    
                    return;
                }

                if (payload.CancellableAction != null)
                {
                    payload.CancellableAction(payload.CancellationTokenSource.Token);
                    return;
                }

            }

            void OnLastTaskFinished(WorkItem payload)
            {
                lock (Sync)
                {
                    RunningWorkItem = null;

                    if(payload.Task.Status == TaskStatus.RanToCompletion)
                        LastCompleteTimeUtc = DateTime.UtcNow;

                    if(payload.ShouldContinueWithNextAction)
                        TryInvokeAsync(should_run_to_completion: false);
                }
            }

            public void SetAction(Action action)
            {
                lock (Sync)
                {
                    Action = action;
                    CancellableAction = null;
                }
            }

            public void SetAction(Action<CancellationToken> cancellableAction)
            {
                lock (Sync)
                {
                    Action = null;
                    CancellableAction = cancellableAction;
                }
            }
        }
    }
}
