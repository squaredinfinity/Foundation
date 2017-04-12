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

        public TagWithValue(string key, object value)
        {
            Key = key;
            Value = value;
        }
        
        public string DebuggerDisplay
        {
            get { return $"{Key}:{Value}"; }
        }
    }
}
