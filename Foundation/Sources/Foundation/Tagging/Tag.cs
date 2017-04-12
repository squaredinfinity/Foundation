using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Tagging
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct Tag
    {
        public string Key { get; private set; }

        public Tag(string key)
        {
            Key = key;
        }

        #region Equality + Hash Code

        public override int GetHashCode()
        {
            return Key.ToLowerInvariant().GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals((Tag)other);
        }

        public bool Equals(Tag other)
        {
            return string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Operators

        public static implicit operator string(Tag tk)
        {
            return tk.Key;
        }

        public static implicit operator Tag(string key)
        {
            return new Tag(key);
        }

        #endregion

        public string DebuggerDisplay
        {
            get { return Key; }
        }
    }
}
