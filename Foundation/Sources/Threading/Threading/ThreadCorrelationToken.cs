using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    /// <summary>
    /// Correlation token linked to a specific thread.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ThreadCorrelationToken : ICorrelationToken, IEquatable<ThreadCorrelationToken>
    {
        int ThreadId;

        /// <summary>
        /// Creates a correlation token from current thread.
        /// </summary>
        public static ICorrelationToken FromCurrentThread { get { return new ThreadCorrelationToken(Environment.CurrentManagedThreadId); } }

        /// <summary>
        /// Creates a new correlation token linked to a thread.
        /// </summary>
        /// <param name="threadId">Id of a thread this correlation token will link to</param>
        public ThreadCorrelationToken(int threadId)
        {
            ThreadId = threadId;
        }

        #region Equality + Hash Code

        public override int GetHashCode() => ThreadId.GetHashCode();
        public override bool Equals(object other) => Equals(other as ThreadCorrelationToken);
        public bool Equals(ICorrelationToken other) => Equals(other as ThreadCorrelationToken);

        public bool Equals(ThreadCorrelationToken other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return int.Equals(ThreadId, other.ThreadId);
        }

        #endregion

        public override string ToString() => $"{ThreadId} (thread)";
        public string DebuggerDisplay => ToString();
    }
}
