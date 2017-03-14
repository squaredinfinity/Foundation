using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SquaredInfinity
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class NamedObject
    {
        public string Name { get; private set; }
        public object Object { get; private set; }

        public NamedObject(string name, object obj)
        {
            this.Name = name;
            this.Object = obj;
        }

        public string DebuggerDisplay
        {
            get { return Name; }
        }
    }
}
