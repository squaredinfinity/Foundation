using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Filters
{
    public abstract class Filter : IFilter
    {
        public static readonly IFilter AllowAll = new AllowAllFilter_Implementation();

        class AllowAllFilter_Implementation : Filter
        {
            public override bool Evaluate(IDiagnosticEvent diagnosticEvent, ILoggerName loggerName)
            {
                return true;
            }
        }

        public string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        FilterMode _mode = FilterMode.Default;
        public FilterMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public abstract bool Evaluate(IDiagnosticEvent diagnosticEvent, ILoggerName loggerName);
    }
}
