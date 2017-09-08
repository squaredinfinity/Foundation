using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SquaredInfinity.Win32Api
{
    public static partial class shell32
    {
        /// <summary>
        /// Tests whether the current user is a member of the Administrator's group.
        /// NOTE: if current process is not elevated it will return false, if elevated it will return true. 
        /// It does not tell if proces could be elevated or not, only what the current state is.
        /// </summary>
        /// <returns></returns>
        [DllImport("shell32.dll")]
        public static extern bool IsUserAnAdmin();
    }
}
