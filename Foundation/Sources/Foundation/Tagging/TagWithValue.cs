using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Tagging
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct TagWithValue
    {
        public string Key { get; private set; }
        public object Value { get; private set; }
        public TagType TagType { get; private set; }

        public TagWithValue(string key, object value, TagType tagType)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value;
            TagType = tagType;
        }
        
        string DebuggerDisplay => $"{Key}:{Value} ({TagType})";
    }
}
