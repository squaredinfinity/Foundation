using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Media
{
    public interface IWpfFastWriteableBitmap : IFastWriteableBitmap
    {
        Color GetPixel(int x, int y);
        void SetPixel(int x, int y, Color color);
    }
}
