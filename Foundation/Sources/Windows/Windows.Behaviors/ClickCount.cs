using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SquaredInfinity.Windows.Behaviors
{
    class ClickCount
    {
        int _value;
        public int Value { get { return _value; } }

        public void Increment() { Interlocked.Add(ref _value, 1); }
        public void Reset() { Interlocked.Exchange(ref _value, 0); }
    }
}
