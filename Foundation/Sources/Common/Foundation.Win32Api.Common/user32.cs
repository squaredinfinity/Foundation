using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace SquaredInfinity.Foundation.Win32Api
{
    public partial class user32
    {
        public static bool IsAnyKeyPressed()
        {
            var keyboardState = GetKeyboardState();
            
            // skip mouse related states
            return keyboardState.Skip(8).Any(state => (state & 0x80) != 0);
        }

        /// <summary>
        /// Flash the spacified Window until it recieves focus.
        /// </summary>
        /// <param name="form">The Window to Flash.</param>
        /// <returns></returns>
        public static bool FlashTaskBarUntilFocus(Window window)
        {
            var wip = new WindowInteropHelper(window);

            FLASHWINFO fi = Create_FLASHWINFO(wip.Handle, FlashWindow.FLASHW_ALL | FlashWindow.FLASHW_TIMERNOFG, uint.MaxValue, 0);
            return FlashWindowEx(ref fi);
        }

        static FLASHWINFO Create_FLASHWINFO(IntPtr handle, uint flags, uint count, uint timeout)
        {
            FLASHWINFO fi = new FLASHWINFO();
            fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
            fi.hwnd = handle;
            fi.dwFlags = flags;
            fi.uCount = count;
            fi.dwTimeout = timeout;
            return fi;
        }
        /// <summary>
        /// Flash the specified Window for the specified number of times
        /// </summary>
        /// <param name="form">The Form Window to Flash.</param>
        /// <param name="count">The number of times to Flash.</param>
        /// <returns></returns>
        public static bool FlashTaskbar(Window window, uint count)
        {
            var wip = new WindowInteropHelper(window);

            FLASHWINFO fi = Create_FLASHWINFO(wip.Handle, FlashWindow.FLASHW_ALL, count, 0);
            return FlashWindowEx(ref fi);
        }

        /// <summary>
        /// Start Flashing the specified Window
        /// </summary>
        /// <param name="form">The Window to Flash.</param>
        /// <returns></returns>
        public static bool StartTaskbarFlash(Window window)
        {
            var wip = new WindowInteropHelper(window);

            FLASHWINFO fi = Create_FLASHWINFO(wip.Handle, FlashWindow.FLASHW_ALL, uint.MaxValue, 0);
            return FlashWindowEx(ref fi);
        }
        /// <summary>
        /// Stop Flashing the specified Window in TaskBar
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool StopTaskbarFlash(Window window)
        {
            var wip = new WindowInteropHelper(window);

            FLASHWINFO fi = Create_FLASHWINFO(wip.Handle, FlashWindow.FLASHW_STOP, uint.MaxValue, 0);
            return FlashWindowEx(ref fi);
        }


        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL.GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL.GWL_EXSTYLE, extendedStyle | WindowStylesEx.WS_EX_TRANSPARENT);
        }

        public static void PostMessage(IntPtr hWnd, uint msg, VirtualKeys virtualKey)
        {
            PostMessage(hWnd, msg, (int)virtualKey, 0);
        }

        public static KeyStates GetVirtualKeyState(VirtualKeys key)
        {
            var state = GetKeyState(key);

            if (state == 0x1)
                return KeyStates.Toggled;
            if (state < 0)
                return KeyStates.Down;

            return KeyStates.None;
        }
    }
}
