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

        public DataRequest()
        { }

        public DataRequest(string data, bool isAsync = false, bool isCached = false)
        {
            this.Data = data;
            this.IsAsync = isAsync;
            this.IsCached = isCached;
        }

        public static implicit operator DataRequest(string data)
        {
            return new DataRequest (data, isAsync:false, isCached:false );
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
