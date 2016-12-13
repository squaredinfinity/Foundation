using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class InvocationThrottle : IInvocationThrottle
    {
        readonly object Sync = new object();

        public TimeSpan Min { get; private set; } = TimeSpan.FromMilliseconds(50);
        public TimeSpan? Max { get; private set; } = null;

        public TaskScheduler Scheduler { get; private set; } = TaskScheduler.Default;

        public InvocationThrottleWorkItem CurrentWorkItem { get; private set; } = null;
        public InvocationThrottleWorkItem PendingWorkItem { get; private set; } = null;

        readonly System.Threading.Timer MinTimer;
        DateTime LastMinTimerResetUTC;

        public InvocationThrottle()
            : this(TaskScheduler.Default)
        { }

        public InvocationThrottle(TaskScheduler scheduler)
            : this(scheduler, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(250))
        { }

        public InvocationThrottle(int min)
            : this(TimeSpan.FromMilliseconds(min))
        { }

        public InvocationThrottle(TimeSpan min)
            : this(min, null)
        { }

        public InvocationThrottle(TaskScheduler scheduler, TimeSpan min)
            : this(scheduler, min, null)
        { }

        public InvocationThrottle(int min, int max)
            : this(TimeSpan.FromMilliseconds(min), TimeSpan.FromMilliseconds(max))
        { }

        public InvocationThrottle(TimeSpan min, TimeSpan? max)
            : this(TaskScheduler.Default, min, max)
        { }

        public InvocationThrottle(TaskScheduler scheduler, TimeSpan min, TimeSpan? max)
        {
            Scheduler = scheduler;

            Min = min;
            Max = max;

            MinTimer = new Timer(OnTimer, (object)null, Timeout.Infinite, Timeout.Infinite);
        }

        private void OnTimer(object state)
        {
            lock (Sync)
            {
                var signal_time = DateTime.Now.ToUniversalTime();

                if (LastMinTimerResetUTC >= signal_time)
                {
                    Debug.WriteLine($"Timer Elapsed, but got canceled before it could stop this event from occuring");
                    // Timer Elapsed, but got canceled before it could stop this event from occuring;
                    return;
                }

                Debug.WriteLine($"Timer Elapsed, calling OnNext()");
                OnNext();
            }
        }

        public void Update(TimeSpan min, TimeSpan? max = null)
        {
            lock (Sync)
            {
                Min = min;
                Max = max;
            }
        }

        void MinTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (Sync)
            {
                if (LastMinTimerResetUTC >= e.SignalTime.ToUniversalTime())
                {
                    Debug.WriteLine($"Timer Elapsed, but got canceled before it could stop this event from occuring");
                    // Timer Elapsed, but got canceled before it could stop this event from occuring;
                    return;
                }

                Debug.WriteLine($"Timer Elapsed, calling OnNext()");
                OnNext();
            }
        }

        public void Cancel(bool forced = true)
        {
            lock(Sync)
            {
                if(CurrentWorkItem != null)
                {
                    if (forced || !CurrentWorkItem.MustRunToCompletion)
                    {
                        CurrentWorkItem.CancellationTokenSource.Cancel();
                    }
                }

                if (PendingWorkItem != null)
                    PendingWorkItem = null;
            }
        }

        void OnNext()
        {
            var work_item = (InvocationThrottleWorkItem)null;

            lock (Sync)
            {
                if (CurrentWorkItem != null)
                {
                    Debug.WriteLine($"On Next, Current not null, exit {CurrentWorkItem.Id}");
                    return;
                }

                if (PendingWorkItem == null)
                {
                    Debug.WriteLine($"On Next, No Pending, exit");
                    return;
                }

                var old_current = CurrentWorkItem;

                CurrentWorkItem = PendingWorkItem;
                PendingWorkItem = null;

                work_item = CurrentWorkItem;

                work_item.CancellationTokenSource = new CancellationTokenSource();
            }

            Debug.WriteLine($"On Next, Task.StartNew");

            work_item.Task = Task.Factory.StartNew((_wi) =>
            {
                var wi = (InvocationThrottleWorkItem)_wi;
                var ct = wi.CancellationTokenSource.Token;

                bool success = false;

                try
                {
                    if (ct.IsCancellationRequested)
                    {
                        Debug.WriteLine($"Invoke, canceled before started {wi.Id}");
                    }
                    else
                    {
                        Debug.WriteLine($"Invoke, {wi.Id}");
                        wi.Action(wi.State, wi, ct);
                        Debug.WriteLine($"Invoke success, {wi.Id}");
                        success = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine($"Invoke, canceled {wi.Id}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Invoke, exception {wi.Id}");
                }

                lock (Sync)
                {
                    if (ct.IsCancellationRequested)
                        success = false;

                    var utc_now = DateTime.UtcNow;

                    var old_current = CurrentWorkItem;

                    CurrentWorkItem = null;

                    if (success)
                        wi.CompleteTimeUtc = DateTime.UtcNow;

                    // check if there is a task pending
                    if (PendingWorkItem != null)
                    {
                        // it is, but within min,
                        // or last call failed
                        // reset timer to start from now
                        if (success && utc_now - PendingWorkItem.RequestWindowTimeUTC < Min)
                        {
                            Debug.WriteLine($"Invoke, pending should be triggered by timer, old current: {wi.Id}, pending: {PendingWorkItem.Id}");
                            // nothing to do here, timer will take care of it
                            return;
                        }
                        else
                        {
                            // last call failed
                            // or success but pending is waiting > min

                            // make sure to update window, if current failed
                            if (!success)
                            {
                                if (PendingWorkItem.RequestWindowTimeUTC > old_current.RequestWindowTimeUTC)
                                    PendingWorkItem.RequestWindowTimeUTC = old_current.RequestWindowTimeUTC;

                                // total window past Max waiting period, invoke
                                if (Max != null && PendingWorkItem.RequestTimeUTC - PendingWorkItem.RequestWindowTimeUTC >= Max)
                                {
                                    // past Max waiting time, invoke and ensure runs to completion
                                    PendingWorkItem.MustRunToCompletion = true;
                                }

                                // invoke pending action immediately, since its been waiting long enough
                                Debug.WriteLine($"Invoke, Calling On Next, old current: {wi.Id}, pending: {PendingWorkItem.Id}");
                                OnNext();
                                return;
                            }
                        }
                    }
                    else
                    {
                        // no work items pending
                        // nothing else to do here
                        Debug.WriteLine($"Invoke, no more pending items, do nothing {wi.Id}");
                    }
                }

            }, work_item, CancellationToken.None, TaskCreationOptions.None, Scheduler);
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

        public void InvokeAsync(Action<object, CancellationToken> cancellableActionWithState, object state)
        {
            if (cancellableActionWithState == null)
                throw new ArgumentNullException(nameof(cancellableActionWithState));

            InvokeAsync((_state, _wi, _ct) => cancellableActionWithState(_state, _ct), state);
        }

        public void InvokeAsync(Action<object, InvocationThrottleWorkItem, CancellationToken> cancellableActionWithState, object state)
        {
            if (cancellableActionWithState == null)
                throw new ArgumentNullException(nameof(cancellableActionWithState));

            var invocation_time = DateTime.UtcNow;

            var current_item = CurrentWorkItem;
            if (current_item != null && current_item.RequestTimeUTC >= invocation_time)
                return;

            lock (Sync)
            {
                var wi = new InvocationThrottleWorkItem();
                wi.RequestTimeUTC = invocation_time;
                wi.State = state;
                wi.Action = cancellableActionWithState;

                Debug.WriteLine($"Invoke Async {wi.Id}");

                // Pending request alredy exists
                if (PendingWorkItem != null)
                {
                    // update window request time (to be able to check for max later)
                    wi.RequestWindowTimeUTC = PendingWorkItem.RequestWindowTimeUTC;
                    wi.MustRunToCompletion = PendingWorkItem.MustRunToCompletion;

                    // total window past Max waiting period, invoke
                    if (Max != null && invocation_time - wi.RequestWindowTimeUTC >= Max)
                    {
                        // past Max waiting time, invoke and ensure runs to completion
                        wi.MustRunToCompletion = true;
                        PendingWorkItem = wi;
                        Debug.WriteLine($"Invoke Async, must run to completion {wi.Id}");
                        OnNextIfReady();
                        return;
                    }

                    // within min distance of last pending request
                    if (invocation_time - PendingWorkItem.RequestTimeUTC < Min)
                    {
                        PendingWorkItem = wi;

                        Debug.WriteLine($"Invoke Async, Reset Timer, RequestWindow: {wi.RequestTimeUTC}, req-window: {(wi.RequestTimeUTC - wi.RequestWindowTimeUTC).TotalMilliseconds} ms, {wi.Id}");
                        ResetMinTimer();

                        return;
                    }
                    else // passed min distance, invoke
                    {
                        PendingWorkItem = wi;
                        Debug.WriteLine($"Invoke Async, passed min distance {wi.Id}");
                        OnNextIfReady();
                        return;
                    }
                }
                else // there's no pending request
                {
                    wi.RequestWindowTimeUTC = wi.RequestTimeUTC;
                    PendingWorkItem = wi;
                    Debug.WriteLine($"Invoke Async, no pending requests {wi.Id}");
                    OnNextIfReady();
                }
            }
        }

        void OnNextIfReady()
        {
            lock (Sync)
            {
                if (CurrentWorkItem == null)
                {
                    if (PendingWorkItem.MustRunToCompletion)
                    {
                        Debug.WriteLine($"On Next If Ready, On Next() {PendingWorkItem.Id}");
                        OnNext();
                        return;
                    }
                    else
                    {
                        Debug.WriteLine($"On Next If Ready, Reset Timer {PendingWorkItem.Id}");

                        // wait Min before executing
                        ResetMinTimer();

                        return;
                    }
                }
                else
                {
                    if (CurrentWorkItem.MustRunToCompletion)
                    {
                        Debug.WriteLine($"On Next If Ready, Current Must Run {PendingWorkItem.Id}");
                        // continuation of Current work item will handle further processing
                    }
                    else
                    {
                        // cancel current
                        Debug.WriteLine($"On Next If Ready, Cancel Current {PendingWorkItem.Id}");
                        CurrentWorkItem.CancellationTokenSource.Cancel();
                    }
                }
            }
        }

        void ResetMinTimer()
        {
            lock (Sync)
            {
                MinTimer.Change(Min, Timeout.InfiniteTimeSpan);
                LastMinTimerResetUTC = DateTime.UtcNow;
            }
        }

        public string DebuggerDisplay
        {
            get
            {
                return $"Invocation Throttle, min: {Min.ToString()}";
            }
        }
    }
}
