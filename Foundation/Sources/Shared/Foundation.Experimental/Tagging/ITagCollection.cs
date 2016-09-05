using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Tagging
{
    public interface ITagCollection : IEnumerable<Tag>
    {
        void Add(string tag);
        void Add(string tag, object value);
        void AddRange(string tag, IEnumerable<object> values);
        void AddOrUpdate(string tag, object value, bool overrideOldValues = false);

        void Remove(string tag);

        void AddOrUpdateFrom(ITagCollection other, bool overrideOldValues = false);

        bool Contains(string tag);
        bool Contains(string tag, object value);
        
        IReadOnlyList<object> GetTagValues(string tag);
        bool TryGetTagValues(string tag, out IReadOnlyList<object> value);

        IReadOnlyList<object> this[string tag] { get; }
        IReadOnlyList<object> this[Tag tag] { get; }
    }
}
