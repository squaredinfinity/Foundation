using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors
{
    public class DataCollectionContext
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
            set { _currentProcess = value; }
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
            set { _currentThread = value; }
        }
    }
}
