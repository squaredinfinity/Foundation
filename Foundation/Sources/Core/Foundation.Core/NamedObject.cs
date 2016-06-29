using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation
{
    public class NamedObject
    {
        public string Name { get; private set; }
        public object Object { get; private set; }

        public NamedObject(string name, object obj)
        {
            this.Name = name;
            this.Object = obj;
        }
    }
}
