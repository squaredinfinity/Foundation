using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Clipboard.Windows
{
    public class WindowsClipboardService : ClipboardService
    {
        protected override void DoExecute(IClipboardBuilder builder)
        {
            var state = new ClipboardBuilderState();
            builder.Build(state);
        }

        protected override IClipboardBuilder DoCreateClipboardBuilder()
        {
            return new WindowsClipboardBuilder();
        }
    }
}
