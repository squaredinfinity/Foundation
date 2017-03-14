using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Media.Drawing
{
    public partial class UnsafePixelCanvas
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnsafePixelCanvas()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeManagedResources();
            }

            DisposeUnmanagedResources();
        }

        void DisposeUnmanagedResources()
        { }

        void DisposeManagedResources()
        { }
    }
}
