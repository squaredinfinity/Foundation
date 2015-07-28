using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Sinks
{
    public class TraceSink : Sink, IRawMessageSink
    {
        public override void Write(IDiagnosticEvent de)
        {
            var msg = Formatter.Format(de);

            Trace.Write(msg);
        }

        bool _handlesRawMessages = false;
        public bool HandlesRawMessages
        {
            get { return _handlesRawMessages; }
            set { _handlesRawMessages = value; }
        }

        public void Write(string message)
        {
            Trace.Write(message);
        }

        ISinkLocation _sinkLocation = new SinkLocation("virtual", "system.diagnostics.trace");
        public override ISinkLocation SinkLocation
        {
            get { return _sinkLocation; }
        }
    }
}
