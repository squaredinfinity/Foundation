using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Sinks
{
    public class DebugTraceSink : Sink, IRawMessageSink
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
            if (Debugger.IsLogging())
                Debugger.Log(0, (string)null, message);
            else 
                if (message == null)
                OutputDebugString(string.Empty);
            else
                OutputDebugString(message);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);

        ISinkLocation _sinkLocation = new SinkLocation("virtual", "kernel32.outputdebugstring");
        public override ISinkLocation SinkLocation
        {
            get { return _sinkLocation; }
        }
    }
}
