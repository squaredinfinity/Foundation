using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Data
{
    public interface IExecuteOptions
    {
        bool ShouldAsyncOpenConnection { get; }
        bool ShouldAsyncExecuteCommand { get; }
    }
}
