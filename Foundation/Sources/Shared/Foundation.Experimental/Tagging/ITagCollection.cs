using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Tagging
{
    public interface IReadOnlyTagCollection<TTag> : IReadOnlyList<TTag>
        where TTag : IEquatable<TTag>
    {
        bool Contains(TTag tag);
        bool ContainsAny(IReadOnlyList<TTag> tags);
        bool ContainsAll(IReadOnlyList<TTag> tags);
    } 
}
