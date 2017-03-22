using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Data
{
    public interface IExecuteReaderOptions : IExecuteOptions
    {
        int ExpectedResultCount { get; }
    }
}
