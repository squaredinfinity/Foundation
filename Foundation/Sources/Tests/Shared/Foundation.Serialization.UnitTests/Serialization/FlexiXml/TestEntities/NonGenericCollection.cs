using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities
{
    public class NonGenericCollection : List<int>
    {
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
    }
}
