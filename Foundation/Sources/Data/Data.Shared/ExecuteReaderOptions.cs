using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Data
{
    public class ExecuteReaderOptions : ExecuteOptions, IExecuteReaderOptions
    {
        public int ExpectedResultCount { get; set; } = 21;
    }
}
