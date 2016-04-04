using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation
{
    /// <summary>
    /// Provides a DateTime context.
    /// Should be used instead of DateTime.Now for unit testing purposes.
    /// </summary>
    public interface IDateTimeContext : IDisposable
    {
        /// <summary>
        /// Gets the current DateTime
        /// </summary>
        /// <value>The now.</value>
        DateTime Now { get; }
    }
}
