using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Serialization.Mapping
{
    public class FlexiMappedInstance
    {
        public Type MappedType { get; private set; }

        public object Value { get; set; }

        public FlexiMappedInstance(Type mappedType)
        {
            this.MappedType = mappedType;
        }
    }
}
