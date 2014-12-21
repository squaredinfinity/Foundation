using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Media
{
    public unsafe partial class FastWriteableBitmap
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~FastWriteableBitmap()
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
        {
            if (image != null)
            {
                image.UnlockBits(imageData);
                image.Dispose();
                image = null;
            }
        }
    }
}
