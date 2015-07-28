using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Diagnostics;
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
        public void CanAccessGlobalLogger()
        {
            var logger = DiagnosticLogger.Global;

            Assert.IsNotNull(logger);
        }

        [TestMethod]
        public void GlobalLogger__AlwaysReturnsSameInstance()
        {

            var logger = DiagnosticLogger.Global;

            Assert.IsNotNull(logger);

            var logger2 = DiagnosticLogger.Global;

            Assert.AreSame(logger, logger2);
        }

        [TestMethod]
        public void GlobalLogger__CanSetCustomLogger()
        {
            var logger = new DiagnosticLogger("My Custom Logger");

            DiagnosticLogger.SetGlobalLogger(logger);

            var logger2 = DiagnosticLogger.Global;

            Assert.AreEqual(logger, logger2);
        }
    }
}
