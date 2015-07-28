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
    /// <typeparam name="TValue"></typeparam>
    public class MultiMap<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
    {
        public MultiMap()
            : base()
        { }

        public bool Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var values = (HashSet<TValue>) null;

            if (!this.TryGetValue(key, out values))
            {
                values = new HashSet<TValue>();
                base.Add(key, values);
            }

            return values.Add(value);
        }

        public void AddRange(TKey key, IEnumerable<TValue> values)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var old_values = (HashSet<TValue>)null;

            if (!this.TryGetValue(key, out old_values))
            {
                old_values = new HashSet<TValue>();
                base.Add(key, old_values);
            }

            foreach(var new_val in values)
            {
                old_values.Add(new_val);
            }
        }

        public bool ContainsValue(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            bool result = false;

            var values = (HashSet<TValue>)null;

            if (this.TryGetValue(key, out values))
            {
                result = values.Contains(value);
            }

            return result;
        }

        public void RemoveValue(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var values = (HashSet<TValue>) null;

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
        public void ImportFrom(MultiMap<TKey, TValue> other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            foreach (KeyValuePair<TKey, HashSet<TValue>> pair in other)
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
        public HashSet<TValue> GetValues(TKey key)
        {
            var result = (HashSet<TValue>) null;

            if (!base.TryGetValue(key, out result))
            {
                // nothing to do here
            }
            return result;
        }
    }
}