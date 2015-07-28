using SquaredInfinity.Foundation.Diagnostics;
using SquaredInfinity.Foundation.Diagnostics.Sinks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Diagnostics.UnitTests.Common
{
    public class _DiagnosticLoggerTestsContext
    {
        public Moq.MockRepository MoqRepository { get; private set; }

        public Moq.Mock<Sink> SinkMock { get; private set; }

        public ILogger GetDefaultTestLogger()
        {
            MoqRepository = new Moq.MockRepository(Moq.MockBehavior.Default);
            SinkMock = MoqRepository.Create<Sink>();
            SinkMock.CallBase = true;

            var logger = new DiagnosticLogger();

            var config = logger.GetConfigurationClone();

            config.Sinks.Clear();

            config.Sinks.Add(SinkMock.Object);

            logger.ApplyConfiguration(config);

            return logger;
        }
    }
}
