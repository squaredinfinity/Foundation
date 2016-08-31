using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections;

namespace SquaredInfinity.Foundation.Tagging
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class TagCollection : ITagCollection
    {
        readonly MultiMap<string, object> Storage = new MultiMap<string, object>();

        public TagCollection()
            : this (StringComparer.InvariantCultureIgnoreCase)
        { }

        public TagCollection(IEqualityComparer<string> equalityComparer)
        {
            Storage = new MultiMap<string, object>();
        }

        public string DebuggerDisplay
        {
            get
            {
                return string.Join(",", this);
            }
        }

        #region this

        public IReadOnlyList<object> this[string tag]
        {
            get
            {
                return this.GetTagValues(tag);
            }
        }

        public IReadOnlyList<object> this[Tag tag]
        {
            get
            {
                return this.GetTagValues(tag.Key);
            }
        }

        #endregion

        #region Add

        public void AddOrUpdateFrom(ITagCollection other)
        {
            if (other == null)
                return;

            foreach(var tag in other)
            {
                Storage.EnsureKeyExists(tag.Key);
                Storage[tag.Key].Clear();
                Storage[tag.Key].Add(tag.Value);
            }
        }

        public void AddOrUpdate(string tag, object value)
        {
            Storage.EnsureKeyExists(tag);
            Storage[tag].Clear();
            Storage[tag].Add(value);
        }

        public void Add(string tag)
        {
            Storage.Add(tag, Tag.UnspecifiedValue);
        }

        public void Add(string tag, object value)
        {
            Storage.Add(tag, value);
        }

        #endregion

        #region Get Enumerator

        public IEnumerator<Tag> GetEnumerator()
        {
            return
                (from kvp in Storage
                 from v in kvp.Value
                 where !object.Equals(v, Tag.UnspecifiedValue)
                 select new Tag(kvp.Key, v))
                 .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return
                 (from kvp in Storage
                  from v in kvp.Value
                  where !object.Equals(v, Tag.UnspecifiedValue)
                  select new Tag(kvp.Key, v))
                  .GetEnumerator();
        }

        #endregion

        public void Remove(string tag)
        {
            Storage.Remove(tag);
        }

        #region Contains

        public bool Contains(string tag)
        {
            return Storage.ContainsKey(tag);
        }

        public bool Contains(string tag, object value)
        {
            var values = (HashSet<object>)null;

            if (!Storage.TryGetValue(tag, out values))
                return false;

            return values.Contains(value);
        }

        #endregion

        #region (Try) Get Tag Values

        public IReadOnlyList<object> GetTagValues(string tag)
        {
            return Storage[tag].Except(Tag.UnspecifiedValue).ToArray();
        }

        public bool TryGetTagValues(string tag, out IReadOnlyList<object> values)
        {
            var hs = (HashSet<object>)null;

            if (!Storage.TryGetValue(tag, out hs))
            {
                values = null;
                return false;
            }

            values = hs.Except(Tag.UnspecifiedValue).ToArray();
            return true;
        }

        #endregion
    }
}
