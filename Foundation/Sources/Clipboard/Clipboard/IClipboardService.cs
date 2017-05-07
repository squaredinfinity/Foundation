using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Clipboard
{
    /// <summary>
    /// Abstracted interactions with clipboard.
    /// </summary>
    public interface IClipboardService
    {
        /// <summary>
        /// Creates clipboard builder which is used to prepare clipboard object.
        /// </summary>
        /// <returns></returns>
        IClipboardBuilder CreateClipboardBuilder();

        /// <summary>
        /// Applies clipbaord object created by clipboard bulder.
        /// </summary>
        /// <param name="builder"></param>
        void Execute(IClipboardBuilder builder);
    }
}
