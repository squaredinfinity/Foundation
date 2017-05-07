using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Clipboard
{
    public class ClipboardBuilderState
    {
        public Dictionary<string, object> Properties { get; private set; } = new Dictionary<string, object>();
    }
}
