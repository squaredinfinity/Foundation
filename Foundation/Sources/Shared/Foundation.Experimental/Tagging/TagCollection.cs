using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Collections;

namespace SquaredInfinity.Foundation.Tagging
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class TagCollection : ITagCollection
    {
        readonly Dictionary<string, Tag> Storage = new Dictionary<string, Tag>();

        public TagCollection()
            : this (StringComparer.InvariantCultureIgnoreCase)
        { }

        public TagCollection(IEqualityComparer<string> equalityComparer)
        {
            Storage = new Dictionary<string, Tag>(equalityComparer);
        }

        public string DebuggerDisplay
        {
            get
            {
                return string.Join(",", this);
            }
        }

        public void AddOrUpdateFrom(ITagCollection other)
        {
            if (other == null)
                return;

            foreach(var tag in other)
            {
                Storage[tag.Key] = tag;
            }
        }

        public void AddOrUpdate(string tag, object value)
        {
            Storage[tag] = new Tag(tag, value);
        }

        public void Add(string tag)
        {
            if (!Storage.ContainsKey(tag))
                Storage.Add(tag, new Tag(tag));
        }

        public IEnumerator<Tag> GetEnumerator()
        {
            return Storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Storage.GetEnumerator();
        }

        public void Remove(string tag)
        {
            Storage.Remove(tag);
        }

        public bool Contains(string tag)
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
