using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Diagnostics
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct SeverityLevel
    {
        byte _value;
        public byte Value
        {
            get { return _value; }
            set { _value = value; }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string DebuggerDisplay
        {
            get
            {
                return "{0} ({1})".FormatWith(Name, Value);
            }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is SeverityLevel && this == (SeverityLevel)obj;
        }

        public static bool operator ==(SeverityLevel x, SeverityLevel y)
        {
            return x.Value == y.Value;
        }

        public static bool operator !=(SeverityLevel x, SeverityLevel y)
        {
            return x.Value != y.Value;
        }

        public static bool operator >=(SeverityLevel x, SeverityLevel y)
        {
            return x.Value >= y.Value;
        }

        public static bool operator <(SeverityLevel x, SeverityLevel y)
        {
            return x.Value < y.Value;
        }

        public static bool operator >(SeverityLevel x, SeverityLevel y)
        {
            return x.Value > y.Value;
        }

        public static bool operator <=(SeverityLevel x, SeverityLevel y)
        {
            return x.Value <= y.Value;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
