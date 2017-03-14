using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using System.Reflection;
using System.IO;

namespace SquaredInfinity.Diagnostics.Configuration.Providers
{
    public class DefaultConfigurationProvider : IConfigurationProvider
    {
        readonly string RequestedEnvironmentName;

        public DefaultConfigurationProvider()
        {
            this.RequestedEnvironmentName = null;
        }

        public DefaultConfigurationProvider(string environmentName)
        {
            this.RequestedEnvironmentName = environmentName;
        }

        public IDiagnosticsConfiguration LoadConfiguration()
        {
            var environmentName = (string)null;

            if (!RequestedEnvironmentName.IsNullOrEmpty())
            {
                environmentName = RequestedEnvironmentName;
            }
            else
            {
                if (!TryDetectEnvironment(out environmentName))
                {
                    environmentName = "unspecified";
                    InternalTrace.Warning(() => "Unable to detect environment in which this code is running. Loading environment-agnostic settings.");
                }
            }

            //# get default config from resource in this assemly
            var asm = Assembly.GetExecutingAssembly();
            var resourceName = $@"SquaredInfinity.Diagnostics.Resources.Default_Configs.config.{environmentName}.xml";

            using (var configStream = asm.GetManifestResourceStream(resourceName))
            {
                using (StreamReader sr = new StreamReader(configStream))
                {
                    var configXml = sr.ReadToEnd();

                    var xmlConfigProvider = new XmlConfigurationProvider(configXml);

                    return xmlConfigProvider.LoadConfiguration();
                }
            }
        }

        /// <summary>
        /// Detects if current environment is desktop, windows rt, winfows phone, azure, asp.net, etc.
        /// </summary>
        /// <param name="environmentName"></param>
        /// <returns></returns>
        bool TryDetectEnvironment(out string environmentName)
        {
            environmentName = "unspecified";

            //# Unit Test

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                var domainName = AppDomain.CurrentDomain.FriendlyName;

                if (domainName.StartsWith("UnitTestAdapter"))
                {
                    environmentName = "unit-test";
                    return true;
                }
            }

            //# Console
            if (Console.In != System.IO.TextReader.Null)
            {
                environmentName = "windows.console";
                return true;
            }

            //# WPF

            //if (System.Windows.Application.Current != null)
            //{
            //    environmentName = "wpf";
            //    return true;
            //}

            //# WinForms 

            //# Windows Phone

            //# Azure (Web?)

            //# ASP.Net

            //# Windows Service

            //# Click Once 

            return false;
        }
    }
}
