using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;

namespace SquaredInfinity.Clipboard
{
    public class WindowsClipboardService : ClipboardService
    {
        protected override IClipboardBuilder DoGetClipboardBuilder()
        {
            return new WindowsClipboardBuilder(this);
        }
    }
}
