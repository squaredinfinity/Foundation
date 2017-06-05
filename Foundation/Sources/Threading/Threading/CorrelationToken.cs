using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public interface ICorrelationToken : IEquatable<ICorrelationToken>
    { }

    public class ThreadCorrelationToken : ICorrelationToken, IEquatable<ThreadCorrelationToken>
    {
        int ThreadId;

        public static ICorrelationToken FromCurrentThread { get { return new ThreadCorrelationToken(Environment.CurrentManagedThreadId); } }

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
    }

    public class CorrelationToken : ICorrelationToken, IEquatable<CorrelationToken>
    {
        Guid Id;
        string Name;

        public CorrelationToken()
            : this("") { }

        public CorrelationToken(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }

        #region Equality + Hash Code

        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object other) => Equals(other as CorrelationToken);
        public bool Equals(ICorrelationToken other) => Equals(other as CorrelationToken);

        public bool Equals(CorrelationToken other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Guid.Equals(Id, other.Id);
        }

        #endregion

        public override string ToString() => $"{Name} {Id}";

        public static implicit operator CorrelationToken (string name) => new CorrelationToken(name);
    }
}
