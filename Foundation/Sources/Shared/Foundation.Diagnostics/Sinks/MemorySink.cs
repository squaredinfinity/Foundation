using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Sinks
{
    /// <summary>
    /// Stores diagnostic events in memory.
    /// </summary>
    /// <para>
    /// Use <see cref="GetEvents"/> method to get list of events.
    /// Use <see cref="ClearEvents"/> method to delete all events.
    /// </para>
    public class MemorySink : Sink
    {
        readonly IDiagnosticEventCollection _events = new DiagnosticEventCollection();
        IDiagnosticEventCollection Events
        {
            get { return _events; }
        }

        public override void Write(IDiagnosticEvent de)
        {
            Events.Add(de);
        }

        public IReadOnlyList<IDiagnosticEvent> GetEvents()
        {
            return Events.ToList().AsReadOnly();
        }

        public void Clear()
        {
            Events.Clear();
        }

        SinkLocation _sinkLocation = new SinkLocation("virtual", "squaredinfinity.diagnostics.sinks.memorysink");
        public override ISinkLocation SinkLocation
        {
            get { return _sinkLocation; }
        }
    }
}
