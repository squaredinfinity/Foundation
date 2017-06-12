using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    /// <summary>
    /// Generic correlation token with unique Id which can be given a name.
    /// </summary>
    public class CorrelationToken : ICorrelationToken, IEquatable<CorrelationToken>
    {
        Guid Id;
        string DisplayName;

        #region Constructors

        /// <summary>
        /// Creates new correlation token a with random id.
        /// </summary>
        public CorrelationToken()
            : this("__noname__", Guid.NewGuid()) { }

        /// <summary>
        /// Creates a new correlation token with a given display name and a random id.
        /// </summary>
        /// <param name="displayName">Display name of the correlation token</param>
        public CorrelationToken(string displayName)
            : this(displayName, Guid.NewGuid()) { }

        /// <summary>
        /// Creates a new correlation token with a given id.
        /// </summary>
        /// <param name="id">id of the correlation token</param>
        public CorrelationToken(Guid id)
            : this("__noname__", id) { }

        /// <summary>
        /// Creates a new correlation token with a given display name and id.
        /// </summary>
        /// <param name="displayName">Display name of the correlation token</param>
        /// <param name="id">id of the correlation token</param>
        public CorrelationToken(string displayName, Guid id)
        {
            DisplayName = displayName;
            Id = id;
        }

        #endregion

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

        public override string ToString() => $"{DisplayName} {Id}";

        public static implicit operator CorrelationToken (string name) => new CorrelationToken(name);
    }
}
