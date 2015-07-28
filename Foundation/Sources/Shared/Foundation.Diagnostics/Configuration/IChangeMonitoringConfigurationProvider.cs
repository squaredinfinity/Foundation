using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Configuration
{
    public interface IChangeMonitoringConfigurationProvider : IConfigurationProvider
    {
        event EventHandler<EventArgs> AfterConfigurationUpdated;

        void StartChangeMonitoring();

        void StopChangeMonitoring();
    }
}
