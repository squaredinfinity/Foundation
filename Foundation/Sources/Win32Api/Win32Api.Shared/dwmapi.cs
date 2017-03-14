using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace SquaredInfinity.Win32Api
{
    public partial class dwmapi
    {
        public static void EnableCustomWindowPreview(Window window, bool enable = true)
        {
            var interopHelper = new WindowInteropHelper(window);

            var hwnd = interopHelper.EnsureHandle();

            EnableCustomWindowPreview(hwnd, enable);
        }

        /// <summary>
        /// Call this method to enable custom previews on the taskbar and task switcher (alt+tab)
        /// The method will call DwmSetWindowAttribute for the specific window handle and let DWM know that we will be providing a custom bitmap for the thumbnail
        /// as well as Aero peek.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="enable"></param>
        public static void EnableCustomWindowPreview(IntPtr hwnd, bool enable = true)
        {
            IntPtr shouldEnable = Marshal.AllocHGlobal(4);

            if (enable)
                Marshal.WriteInt32(shouldEnable, 1);
            else
                Marshal.WriteInt32(shouldEnable, 0);

            try
            {
                int result = DwmSetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE.DWMWA_HAS_ICONIC_BITMAP, shouldEnable, 4);
                if (result != 0)
                {
                    throw Marshal.GetExceptionForHR(result);
                }

                result = DwmSetWindowAttribute(hwnd, DWMWINDOWATTRIBUTE.DWMWA_FORCE_ICONIC_REPRESENTATION, shouldEnable, 4);
                if (result != 0)
                {
                    throw Marshal.GetExceptionForHR(result);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(shouldEnable);
            }
        }
    }
}
