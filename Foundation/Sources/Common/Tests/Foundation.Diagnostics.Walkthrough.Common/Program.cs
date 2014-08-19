using SquaredInfinity.Foundation.Diagnostics;
using SquaredInfinity.Foundation.Diagnostics.Formatters;
using SquaredInfinity.Foundation.Diagnostics.Sinks.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Diagnostics.Walkthrough.Common
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = DiagnosticLogger.Global.GetConfigurationClone();

            var fileSink = new FileSink();
            fileSink.FileNamePattern = "log.txt";
            fileSink.FileLocationPattern = AppDomain.CurrentDomain.BaseDirectory + @"\Logs\";
            fileSink.Formatter = new RawFormatter();

            config.Sinks.Add(fileSink);

            DiagnosticLogger.Global.ApplyConfiguration(config);

            DiagnosticLogger.Global.Information("Let's start!");
            DiagnosticLogger.Global.Warning("Let's start!");
            DiagnosticLogger.Global.Critical("Let's start!");
        }
    }
}
