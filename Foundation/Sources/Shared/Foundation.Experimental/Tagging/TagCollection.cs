using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Extensions;
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

        public object this[string tag]
        {
            get
            {
                return this.GetTagValue(tag);
            }
            set
            {
                AddOrUpdate(tag, value);
            }
        }

        public object this[Tag tag]
        {
            get
            {
                return this.GetTagValue(tag.Key);
            }
            set
            {
                this.AddOrUpdate(tag.Key, value);
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
            if (Storage.ContainsKey(tag))
                throw new ArgumentException($"tag with key {tag.ToStringWithNullOrEmpty()} already exists.");
            else
                Storage.Add(tag, new Tag(tag));
        }

        public void Add(string tag, object value)
        {
            if (Storage.ContainsKey(tag))
                throw new ArgumentException($"tag with key {tag.ToStringWithNullOrEmpty()} already exists.");
            else
                Storage.Add(tag, new Tag(tag, value));
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

        public object GetTagValue(string tag)
        {
            return Storage[tag];
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
