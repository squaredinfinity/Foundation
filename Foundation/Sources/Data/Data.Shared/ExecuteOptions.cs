using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Data
{
    public class ExecuteOptions : IExecuteOptions
    {
        public bool ShouldAsyncOpenConnection { get; set; } = false;
        public bool ShouldAsyncExecuteCommand { get; set; } = false;
    }
}
