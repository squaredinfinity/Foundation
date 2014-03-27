using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Description
{
    /// <summary>
    /// Provides an interface for accessing (get, set) members of a type.
    /// </summary>
    public interface IMemberAccessor
    {
        string SanitizedName { get; }
        bool CanGetValue { get; }
        bool CanSetValue { get; }

        void TrySetValue(object newValue);
        object TryGetValue();
    }
}
