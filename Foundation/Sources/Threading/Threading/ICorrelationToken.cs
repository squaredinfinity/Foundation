using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    /// <summary>
    /// Represents a correlation token which can be used to correlate various operations.
    /// </summary>
    public interface ICorrelationToken : IEquatable<ICorrelationToken>
    { }
}
