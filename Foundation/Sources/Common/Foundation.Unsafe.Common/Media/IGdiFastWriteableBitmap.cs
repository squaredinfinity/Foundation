using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media
{
    public interface IGdiFastWriteableBitmap : IFastWriteableBitmap
    {
        Color GetPixel(int x, int y);
        void SetPixel(int x, int y, Color color);
    }
}
