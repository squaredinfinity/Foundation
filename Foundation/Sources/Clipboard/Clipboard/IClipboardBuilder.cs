using SquaredInfinity.Clipboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SquaredInfinity.Clipboard
{
    public interface IClipboardBuilder
    {
        IClipboardService Service { get; }
        
        IClipboardBuilderStep ClearClipboard();
        IClipboardBuilderStep CopyToClipboard();
        IClipboardBuilderStep LinkTo(IPropagatorBlock<IClipboardBuilder, IClipboardBuilder> target);
        IClipboardBuilderStep SetHtml(string html);
    }
}
