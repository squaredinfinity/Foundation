using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media.Drawing
{
    public partial class
#if UNSAFE
        UnsafePixelCanvas
#else
 PixelCanvas
#endif
    {

        public void Clear()
        {
            Clear(0);
        }

        public abstract void Clear(int color);
    }
}