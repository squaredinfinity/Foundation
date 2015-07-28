using SquaredInfinity.Foundation.Diagnostics.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Configuration
{
    public interface IConfigurationRepository
    {
        IFilterCollection Filters { get; }
    }
}
