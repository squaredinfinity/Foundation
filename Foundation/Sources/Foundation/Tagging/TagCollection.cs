using SquaredInfinity.Collections;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Tagging
{
    public class TagCollection : ITagCollection
    {
        readonly ConcurrentDictionary<Tag, object> Storage = new ConcurrentDictionary<Tag, object>();

        public TagCollection()
        { }

        public TagCollection(IDictionary<string, object> tags)
        {
            foreach (var kvp in tags)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        #region Indexer

        public object this[string tag] => GetAllValues(tag).FirstOrDefault();

        public object this[Tag tag] => GetAllValues(tag).FirstOrDefault();

        #endregion

        #region Add

        public void Add(string tag)
        {
            Add(new Tag(tag));
        }

        public void Add(Tag tag)
        {
            if (Storage.ContainsKey(tag))
                throw new ArgumentException($"Specified tag already exists: {tag.Key}");

            Storage.TryAdd(tag, new MarkerValue());
        }

        public void Add(string tag, object value, TagType tagType = TagType.SingleValue)
        {
            Add(new Tag(tag), value, tagType);
        }

        public void Add(Tag tag, object value, TagType tagType = TagType.SingleValue)
        {
            if (Storage.ContainsKey(tag))
                throw new ArgumentException($"Specified tag already exists: {tag.Key}");

            if (tagType == TagType.SingleValue)
            {
                if (!Storage.TryAdd(tag, value))
                {
                    throw new InvalidOperationException("key already exists.");
                }
            }
            else
            {
                if (!Storage.TryAdd(tag, new MultiValue(value)))
                {
                    throw new InvalidOperationException("key already exists.");
                }
            }
        }

        #endregion

        public void AddOrAppend(string tag, object value)
        {
            if (!Storage.TryGetValue(tag, out var v))
            {
                // value does not exist, try add
                if (Storage.TryAdd(tag, new MultiValue(value)))
                {
                    // added, can quit now
                    return;
                }
                else
                {
                    // must have been added already,
                    // ignore and continue as usual
                }
            }


            var mv = v as MultiValue;

            if (mv == null)
            {
                throw new InvalidOperationException($"Tag does not support appending. {tag}");
            }

            mv.Add(value);
        }

        public T AddOrUpdate<T>(string tag, T value)
        {
            Storage[tag] = new MultiValue(value);

            return value;
        }

        public object AddOrUpdate(string tag, Func<Tag, object> addValueFactory, Func<Tag, object, object> updateValueFactory)
        {
            return Storage.AddOrUpdate(tag, addValueFactory, updateValueFactory);
        }

        public T AddOrUpdate<T>(string tag, Func<Tag, T> addValueFactory, Func<Tag, T, T> updateValueFactory)
        {
            return (T) Storage.AddOrUpdate(tag, (_key) => addValueFactory(_key), (_key, _old) => updateValueFactory(_key, (T)_old));
        }

        public void AddOrUpdateFrom(ITagCollection other, MissingTagBehavior missingTagUpdateBehavior = MissingTagBehavior.Keep)
        {
            // foreach value in this, if it's MarkerValue and does not exist in other -> remove it
            // same for tags not in other if missing tag update behavior is Remove
            // 
            // do first, before adding any new values from other to potentially make this loop shorter
            foreach (var twv in this.GetAllRawValues())
            {
                if (!other.Contains(twv.Key))
                {
                    if (twv.Value is MarkerValue || missingTagUpdateBehavior == MissingTagBehavior.Remove)
                        Storage.TryRemove(twv.Key, out _);
                }
            }

            // add or override values
            foreach (var twv in other.GetAllRawValues())
            {
                if(Storage.TryGetValue(twv.Key, out var v))
                {
                    var mv = v as MultiValue;

                    if (mv != null)
                    {
                        mv.Clear();
                        mv.Add(twv.Value);
                    }
                    else
                        Storage[twv.Key] = twv.Value;
                }
                else
                {
                    Storage[twv.Key] = twv.Value;
                }
            }
        }

        public void AddUpdateOrAppendFrom(ITagCollection other, MissingTagBehavior missingTagUpdateBehavior = MissingTagBehavior.Keep)
        {
            // foreach value in this, if it's MarkerValue and does not exist in other -> remove it
            // same for tags not in other if missing tag update behavior is Remove
            // 
            // do first, before adding any new values from other to potentially make this loop shorter
            foreach (var twv in this.GetAllRawValues())
            {
                if (!other.Contains(twv.Key))
                {
                    if (twv.Value is MarkerValue || missingTagUpdateBehavior == MissingTagBehavior.Remove)
                        Storage.TryRemove(twv.Key, out _);
                }
            }

            foreach (var twv in other.GetAllRawValues())
            {
                if (twv.Value is MultiValue)
                {
                    var mv = (twv.Value as MultiValue);

                    foreach(var v in mv.GetAll())
                        AddOrAppend(twv.Key, v);
                }
                else
                {
                    Storage[twv.Key] = twv.Value;
                }
            }
        }

        #region Clear

        public void Clear()
        {
            Storage.Clear();
        }

        #endregion

        #region Contains

        public bool Contains(string tag)
        {
            return Storage.ContainsKey(tag);
        }

        public bool Contains(Tag tag)
        {
            return Storage.ContainsKey(tag);
        }

        public bool Contains(string tag, object value)
        {
            return Contains(new Tag(tag), value);
        }

        public bool Contains(Tag tag, object value)
        {
            if (!Storage.TryGetValue(tag, out var v))
                return false;

            if (v is MarkerValue)
                return bool.Equals(value, true);

            if(v is MultiValue)
            {
                return (v as MultiValue).GetAll().Contains(value);
            }

            return object.Equals(v, value);
        }

        #endregion

        #region Get All Values

        public IReadOnlyList<object> GetAllValues(string tag)
        {
            return GetAllValues(new Tag(tag));
        }

        public IReadOnlyList<object> GetAllValues(Tag tag)
        {
            if (!Storage.TryGetValue(tag, out var v))
                return new object[0];

            if (v is MarkerValue)
                return new object[] { true };

            var mv = v as MultiValue;
            if (mv != null)
            {
                return mv.GetAll();
            }

            return new object[] { v };
        }

        #endregion

        [Obsolete("Use .Get() instead.")]
        public T GetValue<T>(string tag, Func<T> defaultValue)
        {
            if (Storage.TryGetValue(tag, out var o))
            {
                return (T)o;
            }
            else
            {
                return defaultValue();
            }
        }

        public bool TryGet<T>(string tag, out T value)
        {
            if(Storage.TryGetValue(tag, out var o))
            {
                value = (T)o;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public bool TryGet<T>(Tag tag, out T value)
        {
            if (Storage.TryGetValue(tag, out var o))
            {
                value = (T)o;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }


        public T Get<T>(string tag, Func<T> defaultValue)
        {
            if (Storage.TryGetValue(tag, out var o))
            {
                return (T)o;
            }
            else
            {
                return defaultValue();
            }
        }

        public T Get<T>(Tag tag, Func<T> defaultValue)
        {
            if (Storage.TryGetValue(tag, out var o))
            {
                return (T)o;
            }
            else
            {
                return defaultValue();
            }
        }

        public T GetOrAdd<T>(string tag, Func<T> valueFactory)
        {
            return (T)Storage.GetOrAdd(tag, (key) => valueFactory());
        }

        public T GetOrAdd<T>(Tag tag, Func<T> valueFactory)
        {
            return (T)Storage.GetOrAdd(tag, (key) => valueFactory());
        }

        public IReadOnlyList<TagWithValue> GetAllRawValues()
        {
            return 
                (from kvp in Storage
                 select new TagWithValue(kvp.Key, kvp.Value))
                 .ToArray();
        }

        #region Remove

        public void Remove(string tag)
        {
            Storage.TryRemove(tag, out _);
        }

        public void Remove(Tag tag)
        {
            Storage.TryRemove(tag, out _);
        }

        public bool TryRemove<T>(string tag, out T value)
        {
            var _ = (object)null;

            if(!Storage.TryRemove(tag, out _))
            {
                value = default(T);
                return false;
            }
            else
            {
                value = (T)_;
                return true;
            }
        }

        public bool TryRemove<T>(Tag tag, out T value)
        {
            var _ = (object)null;

            if (!Storage.TryRemove(tag, out _))
            {
                value = default(T);
                return false;
            }
            else
            {
                value = (T)_;
                return true;
            }
        }

        #endregion

        #region Get Enumerator

        public IEnumerator<TagWithValue> GetEnumerator()
        {
            foreach (var kvp in Storage)
            {
                if (kvp.Value is MarkerValue)
                {
                    yield return new TagWithValue(kvp.Key, true);
                }
                else if (kvp.Value is MultiValue)
                {
                    foreach (var v in (kvp.Value as MultiValue).GetAll())
                        yield return new TagWithValue(kvp.Key, v);
                }
                else
                {
                    yield return new TagWithValue(kvp.Key, kvp.Value);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public string DebuggerDisplay
        {
            get
            {
                return "Tags: " + string.Join(",", this);
            }
        }
    }
}
