using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Data
{
    public interface IExecuteOptions
    {
        CancellationToken CancellationToken { get; }
        TimeSpan Timeout { get; }

        bool ShouldAsyncOpenConnection { get; }
        bool ShouldAsyncExecuteCommand { get; }
    }
}
