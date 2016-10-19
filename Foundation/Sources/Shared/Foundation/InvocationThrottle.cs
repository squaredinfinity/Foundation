using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public partial class InvocationThrottle_OLD
    {
        TimeSpan TimeSpanMin;
        TimeSpan? TimeSpanMax;

        DateTime? LastInvokeRequestUTC = null;
        DateTime? LastInvokeUTC = null;
        DateTime LastTimerResetUTC = DateTime.MinValue;

        TimeSpan _actionTimeout = Timeout.InfiniteTimeSpan;
        public TimeSpan ActionTimout
        {
            get { return _actionTimeout; }
            set
            {
                _actionTimeout = value;
                CurrentActionInfo.Timeout = _actionTimeout;
            }
        }

        readonly ActionInfo CurrentActionInfo = new ActionInfo();

        readonly object Lock = new object();

        System.Timers.Timer ThrottleTimer;

        public InvocationThrottle_OLD(TimeSpan min)
            : this(min, null)
        {}

        public InvocationThrottle_OLD(TimeSpan min, TimeSpan? max)
        {
            ActionTimout = Timeout.InfiniteTimeSpan;

            if (min.Ticks <= 0)
                this.TimeSpanMin = TimeSpan.FromTicks(1);
            else
                this.TimeSpanMin = min;

            this.TimeSpanMax = max;

            InitializeTimer();
        }

        void InitializeTimer()
        {
            ThrottleTimer = new System.Timers.Timer(TimeSpanMin.TotalMilliseconds);
            ThrottleTimer.Elapsed += ThrottleTimer_Elapsed;
        }

        public void Update(TimeSpan min, TimeSpan max)
        {
            lock (Lock)
            {
                TimeSpanMin = min;
                TimeSpanMax = max;

                ThrottleTimer.Interval = TimeSpanMin.TotalMilliseconds;
            }
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
                
                CurrentActionInfo.TryInvokeAsync(should_run_to_completion: false);
                LastInvokeUTC = e.SignalTime.ToUniversalTime();
                LastInvokeRequestUTC = null;
            }
        }

        public void Invoke(Action action)
        {
            DoInvoke(() => CurrentActionInfo.SetAction(action));
        }

        public void Invoke(Action<CancellationToken> cancellableAction)
        {
            DoInvoke(() => CurrentActionInfo.SetAction(cancellableAction));
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
                    (LastInvokeRequestUTC != null && current_invoke_request_time - LastInvokeRequestUTC.Value >= TimeSpanMin);

                bool passed_max_time =
                    LastInvokeRequestUTC != null
                    &&
                    (TimeSpanMax == null
                    ||
                    current_invoke_request_time - LastInvokeUTC >= TimeSpanMax);

                LastInvokeRequestUTC = current_invoke_request_time;

                bool should_invoke =
                    // is past min
                    passed_min_time
                    ||
                    // is past max
                    (TimeSpanMax != null && passed_max_time);

                // if min time passed then check if max time span passed
                if (should_invoke)
                {
                    // if is past max since last invoke request
                    // and is past max since last complete
                    // make sure it finishes before processing further
                    var should_run_to_completion =
                        TimeSpanMax != null
                        &&
                        (current_invoke_request_time - LastInvokeUTC >= TimeSpanMax
                        ||
                        current_invoke_request_time - CurrentActionInfo.LastCompleteTimeUtc >= TimeSpanMax);

                    // invoke, stop the timer, update times
                    CurrentActionInfo.TryInvokeAsync(should_run_to_completion);

                    LastInvokeUTC = current_invoke_request_time;
                    ThrottleTimer.Stop();
                    //LastInvokeRequestUTC = current_invoke_request_time;
                    LastInvokeRequestUTC = null;

                    return;
                }

                // restart the timer
                LastTimerResetUTC = current_invoke_request_time;
                ThrottleTimer.Stop();
                ThrottleTimer.Start();
            }
        }
    }
}
