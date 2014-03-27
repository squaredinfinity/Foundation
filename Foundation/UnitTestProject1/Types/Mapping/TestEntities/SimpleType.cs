using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1.Types.Mapping.TestEntities
{
    public class SimpleType
    {
        public int IntegerProperty { get; set; }
        public string StringProperty { get; set; }
        public DayOfWeek EnumProperty { get; set; }
    }
}
