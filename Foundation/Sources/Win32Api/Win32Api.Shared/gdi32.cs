using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SquaredInfinity.Win32Api
{
    public static class gdi32
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
