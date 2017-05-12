using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Tagging
{
    public interface ITagCollection : IEnumerable<TagWithValue>
    {
        // NOTE:
        // Careful when using generics, especially for methods manipulating existing values
        // For example AddOrUpdate should not use generics because old value may not be of same type as updated value

        #region Add

        /// <summary>
        /// Adds specified a specified tag.
        /// key-only tags can be used as markers (i.e. IsVisible tag)
        /// </summary>
        /// <param name="tag"></param>
        void Add(string tag);

        /// <summary>
        /// Adds specified a specified tag.
        /// key-only tags can be used as markers (i.e. IsVisible tag)
        /// </summary>
        /// <param name="tag"></param>
        void Add(Tag tag);

        /// <summary>
        /// Adds specified tag and value.
        /// It will throw if tag already exist, see AddOrUpdate()
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        void Add(string tag, object value, TagType tagType = TagType.SingleValue);

        /// <summary>
        /// Adds specified tag and value.
        /// It will throw if tag already exist, see AddOrUpdate()
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        void Add(Tag tag, object value, TagType tagType = TagType.SingleValue);

        #endregion

        /// <summary>
        /// Adds specified tag if doesn't already exists,
        /// otherwise replaces old value with new.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        T AddOrUpdate<T>(string tag, T value);

        object AddOrUpdate(string tag, Func<Tag, object> addValueFactory, Func<Tag, object, object> updateValueFactory);
        T AddOrUpdate<T>(string tag, Func<Tag, T> addValueFactory, Func<Tag, T, T> updateValueFactory);

        /// <summary>
        /// Adds specified tag if doesn't already exist,
        /// otherwise adds provided value to the existing tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        void AddOrAppend(string tag, object value);

        /// <summary>
        /// Adds tags from other collection.
        /// If tags already exist, they will be replaced with values from another collection.
        /// </summary>
        /// <param name="other"></param>
        void AddOrUpdateFrom(ITagCollection other, MissingTagBehavior missingTagUpdateBehavior);

        /// <summary>
        /// Adds tags from other collection.
        /// If tag already exists but supports single value, it gets updated.
        /// If tag already exists but supports multiple values, new values get appended.
        /// </summary>
        /// <param name="other"></param>
        void AddUpdateOrAppendFrom(ITagCollection other, MissingTagBehavior missingTagUpdateBehavior);

        #region Clear

        void Clear();

        #endregion

        #region Remove

        /// <summary>
        /// Removes specified tag
        /// </summary>
        /// <param name="tag"></param>
        void Remove(string tag);

        /// <summary>
        /// Removes specified tag
        /// </summary>
        /// <param name="tag"></param>
        void Remove(Tag tag);

        bool TryRemove<T>(string tag, out T value);
        bool TryRemove<T>(Tag tag, out T value);

        #endregion

        #region Contains

        bool Contains(string tag);
        bool Contains(Tag tag);
        bool Contains(string tag, object value);
        bool Contains(Tag tag, object value);

        #endregion

        #region Get All Values

        IReadOnlyList<object> GetAllValues(string tag);
        IReadOnlyList<object> GetAllValues(Tag tag);

        #endregion

        IReadOnlyList<TagWithValue> GetAllRawValues();

        #region Indexer

        object this[string tag] { get; }
        object this[Tag tag] { get; }

        #endregion


        [Obsolete("Use .Get() instead.")]
        T GetValue<T>(string tag, Func<T> defaultValue);

        T Get<T>(string tag, Func<T> defaultValue);
        T Get<T>(Tag tag, Func<T> defaultValue);
        T GetOrAdd<T>(Tag tag, Func<T> valueFactory);
    }
}
