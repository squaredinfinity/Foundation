using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SquaredInfinity.Clipboard
{
    public static class ClipboardBuilderFluidInterface
    {
        public static IClipboardBuilderStep ClearClipboard(this IClipboardBuilderStep lastStep)
        {
            return lastStep.Builder.ClearClipboard();
        }

        public static IClipboardBuilderStep CopyToClipboard(this IClipboardBuilderStep lastStep)
        {
            return lastStep.Builder.CopyToClipboard();
        }

        public static IClipboardBuilderStep SetHtml(this IClipboardBuilderStep lastStep, string html)
        {
            return lastStep.Builder.SetHtml(html);
        }

        public static IClipboardBuilderStep LinkTo(this IClipboardBuilderStep lastStep, IPropagatorBlock<IClipboardBuilder, IClipboardBuilder> target)
        {
            return lastStep.Builder.LinkTo(target);
        }
    }
}
