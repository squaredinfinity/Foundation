using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Cache
{
    public class CacheItemPolicy
    {
        public DateTimeOffset AbsoluteExpiration { get; set; }
        public TimeSpan SlidingExpiration { get; set; }
    }
}
