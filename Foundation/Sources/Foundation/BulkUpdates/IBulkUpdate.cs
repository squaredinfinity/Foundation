using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public interface IBulkUpdate : IDisposable
    {
        /// <summary>
        /// True if bulk update has started succesfully,
        /// False otherwise
        /// </summary>
        bool HasStarted { get; }
    }
}
