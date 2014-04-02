using SquaredInfinity.Foundation.Types.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Serialization.Mapping
{
    public class FlexiTypeMapper : TypeMapper
    {
        protected override object CreateClonePrototype(Type type)
        {
            return base.CreateClonePrototype(type);
        }
    }
}
