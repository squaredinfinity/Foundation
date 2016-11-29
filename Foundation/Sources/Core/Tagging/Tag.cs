using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SquaredInfinity.Foundation.Tagging
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct Tag : IEquatable<Tag>
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public Tag(string key)
        {
            if (key == null)
                key = "";

            this.Key = key;
            this.Value = true;
        }

        public Tag(string key, object value)
        {
            if (key == null)
                key = "";

            this.Key = key;
            this.Value = value;
        }

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
            // TODO: invariant culture
            return string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase);
        }

        public string DebuggerDisplay
        {
            get { return Key; }
        }
    }
}
