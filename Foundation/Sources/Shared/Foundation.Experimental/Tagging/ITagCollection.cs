using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Tagging
{
    public interface ITagCollection : IEnumerable<Tag>
    {
        void Add(string tag);
        void AddOrUpdate(string tag, object value);
        void Remove(string tag);
        void AddOrUpdateFrom(ITagCollection other);

        bool Contains(string tag);
        bool TryGetTagValue(string tag, out object value);
    }

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
            return string.Equals(Key, other.Key, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
