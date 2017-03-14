using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics
{
    public class AttachedObject : IAttachedObject
    {
        public string Name { get; private set; }
        public object Value { get; private set; }

        public AttachedObject(object value)
        {
            this.Name = "[unnamed]";
            this.Value = value;
        }

        public AttachedObject(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
