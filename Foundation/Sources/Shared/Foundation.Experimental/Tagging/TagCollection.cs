using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SquaredInfinity.Foundation.Tagging
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class TagCollection : TagCollection<string>
    {
        public TagCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        { }

        public string DebuggerDisplay
        {
            get
            {
                return string.Join(",", this);
            }
        }
    }

    

    public class TagCollection<TTag> : ObservableCollectionEx<TTag>, IReadOnlyTagCollection<TTag>
        where TTag : IEquatable<TTag>
    {
        IEqualityComparer<TTag> EqualityComparer = EqualityComparer<TTag>.Default;

        public TagCollection()
        { }

        public TagCollection(IEqualityComparer<TTag> equalityComparer)
        {
            this.EqualityComparer = equalityComparer;
        }
    }
}
