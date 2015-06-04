using System;
using System.Collections.Generic;
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

                LastActionInfo2.InvokeAsync();
                LastActionInfo2 = new LastActionInfo();

                LastInvokeUTC = e.SignalTime.ToUniversalTime();
            }
        }

        LastActionInfo LastActionInfo2 = new LastActionInfo();

        public Task Invoke(Action action)
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

                    LastActionInfo2.InvokeAsync();

                    LastActionInfo2 = new LastActionInfo();

                    LastInvokeUTC = invokeTime;
                    ThrottleTimer.Stop();

                    return LastActionInfo2.InvocationTask;
                }

                // restart the timer
                LastTimerResetUTC = invokeTime;
                ThrottleTimer.Stop();
                ThrottleTimer.Start();

                return LastActionInfo2.InvocationTask;
            }
        }

        class LastActionInfo
        {
            Action Action = null;

            public readonly Task InvocationTask;

            public LastActionInfo()
            {
                InvocationTask = new Task(new Action(() => { }));
            }

            public void InvokeAsync()
            {
                var t = new Task(Action);
                t.ContinueWith(_prev => InvocationTask.RunSynchronously());
                t.Start();
            }

            public Task SetAction(Action action)
            {
                Action = action;
                return InvocationTask;
            }
        }
    }
}
