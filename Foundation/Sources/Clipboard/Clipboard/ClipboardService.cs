using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Clipboard
{
    public abstract class ClipboardService : IClipboardService
    {
        public void Execute(IClipboardBuilder builder) => DoExecute(builder);
        protected abstract void DoExecute(IClipboardBuilder builder);


        public IClipboardBuilder CreateClipboardBuilder() => DoCreateClipboardBuilder();
        protected abstract IClipboardBuilder DoCreateClipboardBuilder();
    }
}
