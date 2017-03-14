using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections;
using SquaredInfinity.Collections;

namespace SquaredInfinity.Tagging
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class TagCollection : ITagCollection
    {
        readonly MultiMap<string, object> Storage = new MultiMap<string, object>();

        public TagCollection()
            : this (StringComparer.OrdinalIgnoreCase) // TODO: Invariant Culture
        { }

        public TagCollection(IEqualityComparer<string> equalityComparer)
        {
            Storage = new MultiMap<string, object>();
        }

        public string DebuggerDisplay
        {
            get
            {
                return "Tags: " + string.Join(",", this);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="overrideOldValues">If true, old tags that also exist in 'other' collection will be removed before new values are imported. Otherwise new values will be added to the set together with old values.</param>
        public void AddOrUpdateFrom(ITagCollection other, bool overrideOldValues = false)
        {
            if (other == null)
                return;

            if(overrideOldValues)
            {
                foreach(var tag in other)
                {
                    if (Storage.ContainsKey(tag.Key))
                        Storage[tag.Key].Clear();
                }
            }

            foreach(var tag in other)
            {
                Storage.EnsureKeyExists(tag.Key);

                Storage[tag.Key].Add(tag.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <param name="overrideOldValues">If true and tag already exists, its values will be removed before new value is imported. Otherwise new value will be added to the set together with old values.</param>
        public void AddOrUpdate(string tag, object value, bool overrideOldValues = false)
        {
            Storage.EnsureKeyExists(tag);

            if (overrideOldValues)
                Storage[tag].Clear();

            Storage[tag].Add(value);
        }

        public void Add(string tag)
        {
            Storage.Add(tag, true);
        }

        public void Add(string tag, object value)
        {
            Storage.Add(tag, value);
        }

        public void AddRange(string tag, IEnumerable<object> values)
        {
            foreach(var v in values)
            {
                Storage.Add(tag, v);
            }
        }

        #endregion

        #region Get Enumerator

        public IEnumerator<Tag> GetEnumerator()
        {
            return
                (from kvp in Storage
                 from v in kvp.Value
                 select new Tag(kvp.Key, v))
                 .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return
                 (from kvp in Storage
                  from v in kvp.Value
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
            if (Storage.ContainsKey(tag))
                return Storage[tag].ToArray();
            else
                return new List<object>();
        }

        public bool TryGetTagValues(string tag, out IReadOnlyList<object> values)
        {
            var hs = (HashSet<object>)null;

            if (!Storage.TryGetValue(tag, out hs))
            {
                values = null;
                return false;
            }

            values = hs.ToArray();
            return true;
        }

        #endregion
    }
}
