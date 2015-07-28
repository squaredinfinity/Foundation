using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class RegistryKeyExtensions
    {
        public static RegistryKey OpenOrCreateSubKey(this RegistryKey rk, string name, bool writable)
        {
            RegistryKey subkey = rk.OpenSubKey(name, writable);

            if (subkey == null)
                subkey = rk.CreateSubKey(name);

            return subkey;
        }
    }
}
