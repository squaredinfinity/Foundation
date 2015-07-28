using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    public class DiagnosticEventProperty : IDiagnosticEventProperty
    {
        string _uniqueName;
        public string UniqueName
        {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        object _value;
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override int GetHashCode()
        {
            return UniqueName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is DiagnosticEventProperty && ((DiagnosticEventProperty)obj).UniqueName == UniqueName;
        }
    }
}
