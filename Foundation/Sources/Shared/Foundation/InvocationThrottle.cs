using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public class InvocationThrottle
    {
        TimeSpan TimeSpanMin;
        TimeSpan? TimeSpanMax;

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
            lock (Lock)
            {
                var invokeTime = DateTime.UtcNow;

                if (LastInvokeUTC == null)
                    LastInvokeUTC = invokeTime;

                LastActionInfo2.SetAction(action);

                if (TimeSpanMax != null && invokeTime - LastInvokeUTC >= TimeSpanMax)
                {
                    // timer max elapsed

                    LastActionInfo2.TryInvokeAsync();

                    LastInvokeUTC = invokeTime;
                    ThrottleTimer.Stop();

                    return;
                }

                // restart the timer
                LastTimerResetUTC = invokeTime;
                ThrottleTimer.Stop();
                ThrottleTimer.Start();
            }
        }

        class ActionInfo
        {
            readonly object Sync = new object();

            Action Action = null;
            Task LastTask = null;
            bool ShouldContinueWinNextAction = false;

            public ActionInfo()
            { }

            public bool TryInvokeAsync()
            {
                lock (Sync)
                {
                    if (Action == null)
                    {
                        return false;
                    }

                    if (LastTask != null)
                    {
                        ShouldContinueWinNextAction = true;
                        return false;
                    }

                    LastTask = new Task(Action);
                    LastTask.ContinueWith(_prev => OnLastTaskFinished(), TaskContinuationOptions.ExecuteSynchronously);
                    LastTask.Start();

                    Action = null;
                    ShouldContinueWinNextAction = false;

                    return true;
                }
            }

            void OnLastTaskFinished()
            {
                lock(Sync)
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
                }
            }
        }
    }
}
