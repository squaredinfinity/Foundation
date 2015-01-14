using SquaredInfinity.Foundation.Diagnostics.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Configuration
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        public IFilterCollection Filters { get; set; }


        public ConfigurationRepository()
        {
            this.Filters = new FilterCollection();
        }
    }
}
