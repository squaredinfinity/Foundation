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
            fileSink.Formatter = new PatternFormatter() { Pattern = 
            @" {NewLine}
            --
        {NewLine}
        [{Event.Level}{?Event.HasCategory=>'\:'}{?Event.HasCategory=>Event.Category} thread:{Thread.Id}] {?Event.HasMessage=>Event.Message}
        {?Event.HasLoggerName=>NewLine}
        {?Event.HasLoggerName=>'Logger\:'} {?Event.HasLoggerName=>Event.LoggerName}
        {?Event.HasCallerInfo=>NewLine}
        {?Event.HasCallerInfo=>'Caller\:'}
        {?Event.HasCallerInfo=>Caller.FullName} {?Event.HasCallerInfo=>Caller.File} {?Event.HasCallerInfo=>'\:'} {?Event.HasCallerInfo=>Caller.LineNumber}
        {?Event.HasException=>NewLine}
        {?Event.HasException=>Event.Exception:dumpWithHeader('Exception')}
        {?Event.HasAdditionalContextData=>NewLine}
        {?Event.HasAdditionalContextData=>Event.AdditionalContextData:dumpWithHeader('Context Data')}
        {?Event.HasAttachedObjects=>NewLine}
        {?Event.HasAttachedObjects=>Event.AttachedObjects:dumpWithHeader('Attached Objects')}"};


            config.Sinks.Add(fileSink);

            DiagnosticLogger.Global.ApplyConfiguration(config);

            DiagnosticLogger.Global.Information("Let's start!");
            DiagnosticLogger.Global.Warning("Let's start!");
            DiagnosticLogger.Global.Critical("Let's start!");
        }
    }
}
