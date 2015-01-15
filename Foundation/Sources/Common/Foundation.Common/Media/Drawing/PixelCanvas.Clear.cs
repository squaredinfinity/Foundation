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

        public void Clear(int color)
        {
#if UNSAFE
throw new NotImplementedException();
#else

            // fill first line ..

            for(int w = 0; w < _width; w++)
            {
                this[w] = color;
            }

            // block copy first line to all remaining lines
            var line = 1;

            while (line < _height)
            {
                Array.ConstrainedCopy(_pixels, 0, _pixels, (_width * line), _width);
                line++;
            }
#endif
        }
    }
}