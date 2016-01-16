using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities
{
    public class SimpleDictionary : Dictionary<int, string>
    {
        public string CustomProperty { get; set; }
    }
}
