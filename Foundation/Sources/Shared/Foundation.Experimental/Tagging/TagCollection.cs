using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Collections;

namespace SquaredInfinity.Foundation.Tagging
{
    public interface ITagCollection : IEnumerable<KeyValuePair<string, object>>
    {
        void AddTag(string tag);
        void AddOrUpdateTag(string tag, object value);
        void RemoveTag(string tag);
        void AddFrom(ITagCollection other);

        bool ContainsTag(string tag);
        bool TryGetTagValue(string tag, out object value);
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class TagCollection : ITagCollection
    {
        readonly Dictionary<string, object> Storage;

        public TagCollection()
            : this (StringComparer.InvariantCultureIgnoreCase)
        { }

        public TagCollection(IEqualityComparer<string> equalityComparer)
        {
            Storage = new Dictionary<string, object>(equalityComparer);
        }

        public string DebuggerDisplay
        {
            get
            {
                return string.Join(",", this);
            }
        }

        public void AddFrom(ITagCollection other)
        {
            if (other == null)
                return;

            foreach(var kvp in other)
            {
                Storage[kvp.Key] = kvp.Value;
            }
        }

        public void AddOrUpdateTag(string tag, object value)
        {
            Storage[tag] = value;
        }

        public void AddTag(string tag)
        {
            if (!Storage.ContainsKey(tag))
                Storage.Add(tag, null);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Storage.GetEnumerator();
        }

        public void RemoveTag(string tag)
        {
            Storage.Remove(tag);
        }

        public bool ContainsTag(string tag)
        {
            return Storage.ContainsKey(tag);
        }

        public bool TryGetTagValue(string tag, out object value)
        {
            if(Storage.ContainsKey(tag))
            {
                value = Storage[tag];
                return true;
            }

            value = null;
            return false;
        }
    }
}
