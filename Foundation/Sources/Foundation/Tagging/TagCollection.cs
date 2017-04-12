﻿using SquaredInfinity.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Tagging
{
    public class TagCollection : ITagCollection
    {
        readonly Dictionary<Tag, object> Storage = new Dictionary<Tag, object>();

        public TagCollection()
        { }

        #region Indexer

        public object this[string tag] => GetAllValues(tag).FirstOrDefault();

        public object this[Tag tag] => GetAllValues(tag).FirstOrDefault();

        #endregion

        public void Add(string tag)
        {
            Add(new Tag(tag));
        }

        public void Add(Tag tag)
        {
            if (Storage.ContainsKey(tag))
                throw new ArgumentException($"Specified tag already exists: {tag.Key}");

            Storage.Add(tag, new MarkerValue());
        }

        public void Add(string tag, object value)
        {
            Add(new Tag(tag), value);
        }

        public void Add(Tag tag, object value)
        {
            if (Storage.ContainsKey(tag))
                throw new ArgumentException($"Specified tag already exists: {tag.Key}");

            Storage.Add(tag, value);
        }

        public void AddOrAppend(string tag, object value)
        {
            if (!Storage.TryGetValue(tag, out var v))
            {
                Storage.Add(tag, new MultiValue(value));
            }
            else
            {
                var mv = v as MultiValue;

                if(mv == null)
                {
                    throw new InvalidOperationException($"Tag does not support appending. {tag}");
                }

                mv.Add(value);
            }
        }

        public void AddOrUpdate(string tag, object value)
        {
            Storage[tag] = new MultiValue(value);
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
                        Storage.Remove(twv.Key);
                }
            }

            // add or override values
            foreach (var twv in other.GetAllRawValues())
            {
                Storage[twv.Key] = twv.Value;
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
                        Storage.Remove(twv.Key);
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
            Storage.Remove(tag);
        }

        public void Remove(Tag tag)
        {
            Storage.Remove(tag);
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
