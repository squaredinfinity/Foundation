using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Tagging
{
    public interface ITagCollection : IEnumerable<Tag>
    {
        void Add(string tag);
        void Add(string tag, object value);
        void AddOrUpdate(string tag, object value);

        void Remove(string tag);

        void AddOrUpdateFrom(ITagCollection other);

        bool Contains(string tag);
        bool Contains(string tag, object value);
        
        IReadOnlyList<object> GetTagValues(string tag);
        bool TryGetTagValues(string tag, out IReadOnlyList<object> value);

        IReadOnlyList<object> this[string tag] { get; }
        IReadOnlyList<object> this[Tag tag] { get; }
    }

    public struct Tag : IEquatable<Tag>
    {
        public static readonly string UnspecifiedValue = "*706afdf6-e67d-4184-b1cc-89859605788b*";

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
