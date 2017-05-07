using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Clipboard
{
    public interface IClipboardBuilderStep
    {
        Action<ClipboardBuilderState> Action { get; }
    }
}
