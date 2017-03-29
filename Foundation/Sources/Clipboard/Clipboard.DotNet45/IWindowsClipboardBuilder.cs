using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Clipboard
{
    public interface IWindowsClipboardBuilder : IClipboardBuilder
    {
        IClipboardBuilderStep SetData(string dataFormat, object data);
    }
}
