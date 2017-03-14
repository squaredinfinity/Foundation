using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Collections
{
    public class OrderedDictionary<TKey, TValue> : 
        IDictionary<TKey, TValue>, 
        ICollection<KeyValuePair<TKey, TValue>>, 
        IEnumerable<KeyValuePair<TKey, TValue>>, 
        IDictionary,
        IEnumerable
    {
        readonly OrderedDictionary InternalDictionary;

        public int Count { get { return InternalDictionary.Count; } }
        public bool IsReadOnly { get { return false; } }

        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                if (InternalDictionary.Contains(key))
                    return (TValue)InternalDictionary[key];
                else
                {
                    var ex = new KeyNotFoundException();
                    // todo: ex.TryAddContext
                    throw ex;
                }
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                else
                    InternalDictionary[key] = value;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return InternalDictionary.Keys.Cast<TKey>().ToArray();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return InternalDictionary.Values.Cast<TValue>().ToArray();
            }
        }
        
        bool IDictionary.IsReadOnly { get { return InternalDictionary.IsReadOnly; } }
        ICollection IDictionary.Keys { get { return InternalDictionary.Keys; } }
        ICollection IDictionary.Values { get { return InternalDictionary.Values; } }

        public OrderedDictionary()
        {
            InternalDictionary = new OrderedDictionary();
        }

        public OrderedDictionary(IDictionary<TKey, TValue> dictionary)
            : this()
        {
            if (dictionary != null)
            {
                foreach (var kvp in dictionary)
                    InternalDictionary.Add(kvp.Key, kvp.Value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            
            InternalDictionary.Add(key, value);
        }

        public void Clear()
        {
            InternalDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null || !InternalDictionary.Contains(item.Key))
                return false;
            else
                return InternalDictionary[item.Key].Equals(item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            
            return this.InternalDictionary.Contains(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");

            if (array.Rank > 1 || arrayIndex >= array.Length || array.Length - arrayIndex < InternalDictionary.Count)
                throw new ArgumentException("array");

            int index = arrayIndex;
            foreach (DictionaryEntry dictionaryEntry in InternalDictionary)
            {
                array[index] = new KeyValuePair<TKey, TValue>((TKey)dictionaryEntry.Key, (TValue)dictionaryEntry.Value);
                index++;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (DictionaryEntry dictionaryEntry in InternalDictionary)
                yield return new KeyValuePair<TKey, TValue>((TKey)dictionaryEntry.Key, (TValue)dictionaryEntry.Value);
        }

        IEnumerator IEnumerable.GetEnumerator() { return (IEnumerator)GetEnumerator(); }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
            {
                InternalDictionary.Remove(item.Key);
                return true;
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (InternalDictionary.Contains(key))
            {
                InternalDictionary.Remove(key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            else
            {
                if(!InternalDictionary.Contains(key))
                {
                    value = default(TValue);
                    return false;
                }
                
                value = (TValue)InternalDictionary[key];
                return true;
            }
        }

        void IDictionary.Add(object key, object value)
        {
            InternalDictionary.Add(key, value);
        }

        void IDictionary.Clear()
        {
            InternalDictionary.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            return InternalDictionary.Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return InternalDictionary.GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            InternalDictionary.Remove(key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.InternalDictionary.CopyTo(array, index);
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return InternalDictionary[(TKey)key];
            }
            set
            {
                InternalDictionary[(TKey)key] = (TValue)value;
            }
        }

        int ICollection.Count
        {
            get { return InternalDictionary.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return InternalDictionary; }
        }
    }
}
