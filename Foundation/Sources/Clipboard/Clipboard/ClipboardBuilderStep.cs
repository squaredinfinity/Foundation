using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Clipboard
{
    public class ClipboardBuilderStep : IClipboardBuilderStep
    {
        public Action<ClipboardBuilderState> Action { get; set; }

        public ClipboardBuilderStep() { }

        public ClipboardBuilderStep(Action<ClipboardBuilderState> action)
        {
            Action = action;
        }
    }
}
