using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Sinks
{
    public interface ISinkCollection : 
        ICollection<ISink>//, 
        //IBulkUpdatesCollection<ISink>, 
        //INotifyCollectionContentChanged
    {
        IReadOnlyList<ISink> MustWaitForWriteSinks { get; }

        IReadOnlyList<ISink> FireAndForgetSinks { get; }

        IReadOnlyList<IRawMessageSink> MustWaitForWriteRawMessageSinks { get; }

        IReadOnlyList<IRawMessageSink> FireAndForgetRawMessageSinks { get; }
    }
}
