using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    class MappingContext
    {
        public readonly ConcurrentDictionary<object, object> Objects_MappedFromTo = new ConcurrentDictionary<object, object>();
    }
}
