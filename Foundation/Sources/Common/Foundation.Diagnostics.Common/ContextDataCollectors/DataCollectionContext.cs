using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors
{
    public class DataCollectionContext : IDataCollectionContext
    {
        Process _currentProcess;
        public Process CurrentProcess
        {
            get
            {
                if (_currentProcess == null)
                    _currentProcess = Process.GetCurrentProcess();

                return _currentProcess;
            }
        }

        Thread _currentThread;
        public Thread CurrentThread
        {
            get
            {
                if (_currentThread == null)
                    _currentThread = Thread.CurrentThread;

                return _currentThread;
            }
        }
    }
}
