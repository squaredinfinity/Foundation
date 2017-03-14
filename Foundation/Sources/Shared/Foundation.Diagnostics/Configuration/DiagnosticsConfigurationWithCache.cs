using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using SquaredInfinity.Diagnostics.Filters;

namespace SquaredInfinity.Diagnostics.Configuration
{
    internal class DiagnosticsConfigurationWithCache : DiagnosticsConfiguration
    {
        public ConfigurationCache Cache { get; private set; }

        public DiagnosticsConfigurationWithCache(IDiagnosticsConfiguration source)
        {
            this.DeepCopyFrom(source);
            RefreshCache();
        }

        public DiagnosticsConfigurationWithCache(ConfigurationCache cache)
        {
            this.Cache = cache;
        }

        internal void RefreshCache()
        {
            if(!Settings.EnableLogging)
            {
                Cache = new ConfigurationCache(allowAll: false);
                return;
            }
            this.Cache = new ConfigurationCache();

            Cache.ShouldProcessCriticals = ShouldProcessSeverityLevel(KnownSeverityLevels.Critical);
            Cache.ShouldProcessErrors = ShouldProcessSeverityLevel(KnownSeverityLevels.Error);
            Cache.ShouldProcessWarnings = ShouldProcessSeverityLevel(KnownSeverityLevels.Warning);
            Cache.ShouldProcessInformation = ShouldProcessSeverityLevel(KnownSeverityLevels.Information);
            Cache.ShouldProcessVerbose = ShouldProcessSeverityLevel(KnownSeverityLevels.Verbose);
        }

        bool ShouldProcessSeverityLevel(SeverityLevel sl)
        {
            // no filters specified,
            // process all severity levels
            if (GlobalFilters.Count == 0)
                return true;

            var result =
                (from f in GlobalFilters.OfType<SeverityFilter>()
                 where f.EvaluateInternal(sl) == true
                 select 1)
                 .Any();

            return result;
        }
    }
}
