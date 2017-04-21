using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Cache
{
    public interface ICacheItemDetails
    {
        DateTime TimeLastUpdatedUtc { get; }
        DateTime TimeToExpireUtc { get; }

        bool TryChangeTimeToExpire(DateTime newTimeToExpireUtc);
    }

    public interface ICacheItemDetails<T>// : ICacheItemDetails
    {
        T Value { get; }

        DateTime TimeAddedToCacheUtc { get; set; }
    }
}
