using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Clipboard
{
    public abstract class ClipboardService : IClipboardService
    {
        public IClipboardBuilder GetClipboardBuilder() => DoGetClipboardBuilder();

        protected abstract IClipboardBuilder DoGetClipboardBuilder();
    }
}
