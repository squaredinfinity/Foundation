using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    /// <summary>
    /// Represents a timeout starting at a specified point in time.
    /// That is not an aboslute timeout, but the actual time left from start time to expiry.
    /// </summary>
    public class TimeoutExpiry
    {
        readonly DateTime StartTime;
        readonly int InitialMillisecondsTimeout;

        /// <summary>
        /// Number of millisends left before the expiry.
        /// </summary>
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

        /// <summary>
        /// True if timeout expired, false otherwise.
        /// </summary>
        public bool HasTimedOut
        {
            get
            {
                if (InitialMillisecondsTimeout == -1)
                    return false;

                return (DateTime.UtcNow - StartTime).TotalMilliseconds > InitialMillisecondsTimeout;
            }
        }

        /// <summary>
        /// Throws exception if timeout has expired.
        /// </summary>
        /// <exception cref="TimeoutException" />
        public void ThrowIfExpired()
        {
            if (HasTimedOut)
                throw new TimeoutException();
        }

        /// <summary>
        /// Creates a new timeout expiry with given number of milliseconds left from now.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        public TimeoutExpiry(int millisecondsTimeout)
        {
            StartTime = DateTime.UtcNow;
            InitialMillisecondsTimeout = millisecondsTimeout;
        }

        /// <summary>
        /// Creates a new timeout expiry with given number of millisceonds left from a specified time.
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        public TimeoutExpiry(DateTime startTime, int millisecondsTimeout)
        {
            StartTime = startTime;
            InitialMillisecondsTimeout = millisecondsTimeout;
        }
    }
}
