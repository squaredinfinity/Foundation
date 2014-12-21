using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media
{
    /// <summary>
    /// Provides a fast way to create and modify bitmaps.
    /// Bitmap is an array of pixels each containing ARGB color value stored as int32 for best performance
    /// </summary>
    public interface IFastWriteableBitmap : IDisposable
    {

    }
}
