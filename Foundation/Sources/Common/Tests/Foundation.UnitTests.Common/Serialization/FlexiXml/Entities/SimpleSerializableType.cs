using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.Entities
{
    public class SimpleSerializableType
    {
        public int IntProperty { get; set; }
        public int? NullableIntProperty { get; set; }
        public string StringProperty { get; set; }
        public Guid GuidProperty { get; set; }
    }
}
