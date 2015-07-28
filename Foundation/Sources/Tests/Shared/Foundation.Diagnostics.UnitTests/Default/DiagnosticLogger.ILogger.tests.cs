using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SquaredInfinity.Foundation.Diagnostics;
using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;
using SquaredInfinity.Foundation.Diagnostics.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Diagnostics.UnitTests.Common.Default
{
    public partial class DiagnosticLoggerTests
    {
        [TestMethod]
        public void Verify__CorrectSeverityLevelsArePassedToSinks()
        {
            var cx = new _DiagnosticLoggerTestsContext();

            var logger = cx.GetDefaultTestLogger();

            var ev = (IDiagnosticEvent)null;

            cx.SinkMock
                .Setup(s => s.Write(It.IsAny<IDiagnosticEvent>()))
                .Callback<IDiagnosticEvent>(_ev => ev = _ev)
                .Verifiable();

            logger.Critical("txt");
            Assert.AreEqual(ev.Severity, KnownSeverityLevels.Critical);
            ev = null;

            logger.CriticalFormat("txt");
            Assert.AreEqual(ev.Severity, KnownSeverityLevels.Critical);
            ev = null;

            logger.Warning("txt");
            Assert.AreEqual(ev.Severity, KnownSeverityLevels.Warning);
            ev = null;

            logger.WarningFormat("txt");
            Assert.AreEqual(ev.Severity, KnownSeverityLevels.Warning);
            ev = null;

            logger.Information("txt");
            Assert.AreEqual(ev.Severity, KnownSeverityLevels.Information);
            ev = null;

            logger.InformationFormat("txt");
            Assert.AreEqual(ev.Severity, KnownSeverityLevels.Information);
            ev = null;

            logger.Verbose("txt");
            Assert.AreEqual(ev.Severity, KnownSeverityLevels.Verbose);
            ev = null;

            logger.VerboseFormat("txt");
            Assert.AreEqual(ev.Severity, KnownSeverityLevels.Verbose);
            ev = null;
        }
    }
}
