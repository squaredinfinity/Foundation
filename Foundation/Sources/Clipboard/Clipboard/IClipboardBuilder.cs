using SquaredInfinity.Clipboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Clipboard
{
    public interface IClipboardBuilder
    {
        void Build(ClipboardBuilderState state);

        void ClearClipboard();
        void CopyToClipboard();
        void SetHtml(string html);
        void SetText(string text);
        /// <summary>
        /// Performs custom action on Clipboard Builder State
        /// </summary>
        /// <param name="action"></param>
        void Custom(Action<ClipboardBuilderState> action);
    }
}
