using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SquaredInfinity.Clipboard
{
    public interface IClipboardBuilderStep
    {
        IClipboardBuilder Builder { get; }
        ISourceBlock<IClipboardBuilder> Block { get; }
    }
}
