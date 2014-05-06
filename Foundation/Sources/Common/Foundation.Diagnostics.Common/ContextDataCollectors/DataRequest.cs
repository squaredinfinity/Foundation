using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors
{
    [DebuggerDisplay("{Data}, IsAsync={IsAsync}, IsCached={IsCached}")]
    public class DataRequest : IDataRequest
    {
        public string Data { get; set; }
        public bool IsAsync { get; set; }
        public bool IsCached { get; set; }

        public static implicit operator DataRequest(string data)
        {
            return new DataRequest { Data = data, IsAsync = false, IsCached = false };
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is DataRequest && ((DataRequest)obj).Data == Data;
        }
    }
}
