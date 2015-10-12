using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public class InvocationThrottle
    {
        TimeSpan TimeSpanMin;
        TimeSpan? TimeSpanMax;

        DateTime? LastInvokeRequestUTC = null;
        DateTime? LastInvokeUTC = null;
        DateTime LastTimerResetUTC = DateTime.MinValue;

        readonly object Lock = new object();

        System.Timers.Timer ThrottleTimer;

        public InvocationThrottle(TimeSpan min)
        {
            if (min.Ticks <= 0)
                this.TimeSpanMin = TimeSpan.FromTicks(1);
            else
                this.TimeSpanMin = min;

            InitializeTimer();
        }

        public InvocationThrottle(TimeSpan min, TimeSpan max)
        {
            this.TimeSpanMin = min;
            this.TimeSpanMax = max;

            InitializeTimer();
        }

        void InitializeTimer()
        {
            ThrottleTimer = new System.Timers.Timer(TimeSpanMin.TotalMilliseconds);
            ThrottleTimer.Elapsed += ThrottleTimer_Elapsed;
        }

        void ThrottleTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (Lock)
            {
                ThrottleTimer.Stop();

                if (LastTimerResetUTC >= e.SignalTime.ToUniversalTime())
                {
                    // Timer Elapsed, but got canceled before it could stop this event from occuring;
                    return;
                }

                LastActionInfo2.TryInvokeAsync();
                LastInvokeUTC = e.SignalTime.ToUniversalTime();
            }
        }

        readonly ActionInfo LastActionInfo2 = new ActionInfo();

        public void Invoke(Action action)
        {
            DoInvoke(() => LastActionInfo2.SetAction(action));
        }

        public void Invoke(Action<CancellationToken> cancellableAction)
        {
            DoInvoke(() => LastActionInfo2.SetAction(cancellableAction));
        }

        void DoInvoke(Action set_action)
        {
            lock (Lock)
            {
                set_action();

                var current_invoke_request_time = DateTime.UtcNow;

                if (LastInvokeUTC == null)
                    LastInvokeUTC = current_invoke_request_time;

                bool passed_min_time =
                    // no min so just invoke
                    TimeSpanMin == TimeSpan.Zero
                    ||
                    // TimeSpanMin passed since last invoke
                    LastInvokeRequestUTC != null && current_invoke_request_time - LastInvokeRequestUTC.Value >= TimeSpanMin;

                // if min time passed then check if max time span passed
                if (passed_min_time)
                {
                    // invoke if max time passed
                    if (TimeSpanMax == null || current_invoke_request_time - LastInvokeUTC >= TimeSpanMax)
                    {
                        // timer max elapsed (or not set)
                        // invoke, stop the timer, update times
                        LastActionInfo2.TryInvokeAsync();

                        LastInvokeUTC = current_invoke_request_time;
                        ThrottleTimer.Stop();
                        LastInvokeRequestUTC = current_invoke_request_time;

                        return;
                    }
                }

                // restart the timer
                LastTimerResetUTC = current_invoke_request_time;
                ThrottleTimer.Stop();
                ThrottleTimer.Start();
            }
        }

        class ActionInfo
        {
            readonly object Sync = new object();

            Action<CancellationToken> CancellableAction = null;
            Action Action = null;

            Task LastTask = null;

            CancellationTokenSource LastTaskCancellationTokenSource;


            bool ShouldContinueWinNextAction = false;

            public ActionInfo()
            { }

            class DoActionPayload
            {
                public Action Action;
                public Action<CancellationToken> CancellableAction;
                public CancellationTokenSource CancellationTokenSource;
            }

            public bool TryInvokeAsync()
            {
                lock (Sync)
                {
                    if (Action == null && CancellableAction == null)
                    {
                        return false;
                    }

                    if (LastTask != null && CancellableAction == null)
                    {
                        ShouldContinueWinNextAction = true;
                        return false;
                    }

                    if (CancellableAction != null)
                    {
                        if (LastTaskCancellationTokenSource != null)
                            LastTaskCancellationTokenSource.Cancel();
                    }


                    LastTaskCancellationTokenSource = new CancellationTokenSource();

                    var payload = new DoActionPayload
                    {
                        Action = Action,
                        CancellableAction = CancellableAction,
                        CancellationTokenSource = LastTaskCancellationTokenSource
                    };

                    LastTask = new Task(DoAction, payload, LastTaskCancellationTokenSource.Token);
                    LastTask.ContinueWith(_prev => OnLastTaskFinished(), TaskContinuationOptions.ExecuteSynchronously);
                    LastTask.Start();

                    Action = null;
                    CancellableAction = null;

                    ShouldContinueWinNextAction = false;

                    return true;
                }
            }

            static void DoAction(object payloadAsObject)
            {
                var payload = (DoActionPayload)payloadAsObject;

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

            void OnLastTaskFinished()
            {
                lock (Sync)
                {
                    LastTask = null;

                    if (!ShouldContinueWinNextAction)
                        return;

                    TryInvokeAsync();
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
