using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Configuration
{
    internal class ConfigurationCache
    {
        public bool ShouldProcessErrors;
        public bool ShouldProcessWarnings;
        public bool ShouldProcessInformation;
        public bool ShouldProcessVerbose;
        public bool ShouldProcessSwallowedExceptions;
        public bool ShouldProcessRawMessages;
        public bool ShouldProcessCriticals;

        public ConfigurationCache()
        { }

        public ConfigurationCache(bool allowAll)
        {
            ShouldProcessErrors = true;
            ShouldProcessWarnings = true;
            ShouldProcessInformation = true;
            ShouldProcessVerbose = true;
            ShouldProcessSwallowedExceptions = true;
            ShouldProcessRawMessages = true;
            ShouldProcessCriticals = true;
        }
    }
}
