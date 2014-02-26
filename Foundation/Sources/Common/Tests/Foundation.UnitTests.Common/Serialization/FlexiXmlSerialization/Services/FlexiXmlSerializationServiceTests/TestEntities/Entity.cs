using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Services.FlexiXmlSerializationServiceTests.TestEntities
{
    public class Entity
    {
        public String StringProperty { get; set; }
        public Int32 Int32Property { get; set; }
        public DayOfWeek EnumProperty { get; set; }
    }
}
