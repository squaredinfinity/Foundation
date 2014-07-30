using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public class MappingContext
    {
        public readonly ConcurrentDictionary<object, object> Objects_MappedFromTo = new ConcurrentDictionary<object, object>();

        public readonly CancellationToken CancellationToken;

        public MappingContext(CancellationToken cancellationToken)
        {
            this.CancellationToken = cancellationToken;
        }
    }
}
