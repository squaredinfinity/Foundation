using SquaredInfinity.Foundation.Diagnostics;
using SquaredInfinity.Foundation.Diagnostics.Fluent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ILoggerExtensions
    {
        static readonly string DiagnosticsAssemblyName = Assembly.GetExecutingAssembly().FullName;


        #region Create Logger For ...


        public static ILogger CreateLoggerForThisType(this ILogger logger, int framesToSkip = 0)
        {
            var stackTrace = new StackTrace(framesToSkip + 1);


            var frame = stackTrace.GetFrame(0);


            var method = frame.GetMethod();


            return CreateLoggerForType(logger, method.DeclaringType);
        }


        public static ILogger CreateLoggerForType<TType>(this ILogger logger)
        {
            return CreateLoggerForType(logger, typeof(TType));
        }


        public static ILogger CreateLoggerForType(this ILogger logger, object instance)
        {
            if (instance == null)
                throw new ArgumentException("instance cannot be null.");


            return CreateLoggerForType(logger, instance.GetType());
        }


        public static ILogger CreateLoggerForType(this ILogger logger, Type type)
        {
            ILogger newLogger = null;


            if (DiagnosticsAssemblyName == type.Assembly.FullName)
            {
                newLogger = new DiagnosticLogger(type.FullName);
            }
            else
            {
                newLogger = new DiagnosticLogger(type.FullName);
                newLogger.Parent = logger as ILogger;
            }


            return newLogger;
        }


        #endregion


        public static DiagnosticEventBuilder AsCritical(this ILogger logger)
        {
            var builder = new DiagnosticEventBuilder(logger);

            builder.EventToBuild.Severity = KnownSeverityLevels.Critical;

            return builder;
        }

        public static DiagnosticEventBuilder AsError(this ILogger logger)
        {
            var builder = new DiagnosticEventBuilder(logger);

            builder.EventToBuild.Severity = KnownSeverityLevels.Error;

            return builder;
        }

        public static DiagnosticEventBuilder AsWarning(this ILogger logger)
        {
            var builder = new DiagnosticEventBuilder(logger);

            builder.EventToBuild.Severity = KnownSeverityLevels.Warning;

            return builder;
        }

        public static DiagnosticEventBuilder AsInformation(this ILogger logger)
        {
            var builder = new DiagnosticEventBuilder(logger);

            builder.EventToBuild.Severity = KnownSeverityLevels.Information;

            return builder;
        }

        public static DiagnosticEventBuilder AsVerbose(this ILogger logger)
        {
            var builder = new DiagnosticEventBuilder(logger);

            builder.EventToBuild.Severity = KnownSeverityLevels.Verbose;

            return builder;
        }
    }
}
