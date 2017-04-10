using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Tagging
{
    public interface ITagCollection : IEnumerable<TagWithValue>
    {
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
        void Add(string tag, object value);

        /// <summary>
        /// Adds specified tag and value.
        /// It will throw if tag already exist, see AddOrUpdate()
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        void Add(Tag tag, object value);

        /// <summary>
        /// Adds specified tag if doesn't already exists,
        /// otherwise replaces old value with new.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <param name="overrideOldValues"></param>
        void AddOrUpdate(string tag, object value);

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
    }
}
