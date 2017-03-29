using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SquaredInfinity.Clipboard
{
    public class ClipboardBuilderStep : IClipboardBuilderStep
    {
        public IClipboardBuilder Builder { get; set; }
        public ISourceBlock<IClipboardBuilder> Block { get; set; }

        public ClipboardBuilderStep() { }

        public ClipboardBuilderStep(IClipboardBuilder builder, ISourceBlock<IClipboardBuilder> block)
        {
            Builder = builder;
            Block = block;
        }
    }
}
