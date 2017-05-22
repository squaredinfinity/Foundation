using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public class TimeoutExpiry
    {
        readonly DateTime StartTime;
        readonly int InitialMillisecondsTimeout;

        public int MillisecondsLeft
        {
            get
            {
                if (InitialMillisecondsTimeout == -1)
                    return -1;

                // return 0 if already timed out
                return Math.Max(0, InitialMillisecondsTimeout - (int)(DateTime.UtcNow - StartTime).TotalMilliseconds);
            }
        }

        public bool HasTimedOut
        {
            get
            {
                if (InitialMillisecondsTimeout == -1)
                    return false;

                return (DateTime.UtcNow - StartTime).TotalMilliseconds > InitialMillisecondsTimeout;
            }
        }

        public TimeoutExpiry(int millisecondsTimeout)
        {
            StartTime = DateTime.UtcNow;
            InitialMillisecondsTimeout = millisecondsTimeout;
        }
    }
}
