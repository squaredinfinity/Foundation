using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1.Types.Mapping.TestEntities
{
    public class NullablesTests
    {
        public class HasNullableProperty
        {
            public int? Property { get; set; }
        }

        public class HasNonNullableProperty
        {
            public int Property { get; set; }
        }
    }
}
