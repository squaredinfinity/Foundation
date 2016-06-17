using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Multimap
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public class MultiMap<TKey, TItem> : Dictionary<TKey, HashSet<TItem>>
    {
        public MultiMap()
            : base()
        { }

        public bool Add(TKey key, TItem value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var values = (HashSet<TItem>) null;

            if (!this.TryGetValue(key, out values))
            {
                values = new HashSet<TItem>();
                base.Add(key, values);
            }

            return values.Add(value);
        }

        public void AddRange(TKey key, IEnumerable<TItem> values)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var old_values = (HashSet<TItem>)null;

            if (!this.TryGetValue(key, out old_values))
            {
                old_values = new HashSet<TItem>();
                base.Add(key, old_values);
            }

            foreach(var new_val in values)
            {
                old_values.Add(new_val);
            }
        }

        public bool ContainsValue(TKey key, TItem value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            bool result = false;

            var values = (HashSet<TItem>)null;

            if (this.TryGetValue(key, out values))
            {
                result = values.Contains(value);
            }

            return result;
        }

        public void RemoveValue(TKey key, TItem value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var values = (HashSet<TItem>) null;

            if (this.TryGetValue(key, out values))
            {
                values.Remove(value);

                if (values.Count <= 0)
                {
                    this.Remove(key);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public void ImportFrom(MultiMap<TKey, TItem> other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            foreach (KeyValuePair<TKey, HashSet<TItem>> pair in other)
            {
                this.AddRange(pair.Key, pair.Value);
            }
        }


        /// <summary>
        /// Returns values associated with specified key.
        /// If no values exists returns null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public HashSet<TItem> GetValues(TKey key)
        {
            var result = (HashSet<TItem>) null;

            if (!base.TryGetValue(key, out result))
            {
                // nothing to do here
            }
            return result;
        }
    }
}