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

        System.Timers.Timer ThrottleTimer;

        public InvocationThrottle(TimeSpan min)
        {
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

        DateTime? LastInvokeUTC = null;

        DateTime LastTimerResetUTC = DateTime.MinValue;

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

                if(LastAction != null)
                    LastAction();

                LastInvokeUTC = e.SignalTime.ToUniversalTime();
                LastAction = null;
            }
        }



        readonly object Lock = new object();

        Action LastAction;

        public void Invoke(Action action)
        {
            lock (Lock)
            {
                var invokeTime = DateTime.UtcNow;

                if (LastInvokeUTC == null)
                    LastInvokeUTC = invokeTime;

                LastAction = action;

                if (TimeSpanMax != null && invokeTime - LastInvokeUTC >= TimeSpanMax)
                {
                    // timer max elapsed
                    LastAction();
                    LastInvokeUTC = invokeTime;
                    ThrottleTimer.Stop();
                    LastAction = null;
                    return;
                }

                // restart the timer
                LastTimerResetUTC = invokeTime;
                ThrottleTimer.Stop();
                ThrottleTimer.Start();
            }
        }
    }
}
