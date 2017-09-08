using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Win32Api
{
    public partial class user32
    {
        [DllImport("user32.dll", EntryPoint = "GetKeyboardState", SetLastError = true)]
        public static extern bool NativeGetKeyboardState([Out] byte[] keyStates);

        public static bool TryGetKeyboardState(out byte[] keyStates)
        {
            keyStates = new byte[256];

            return NativeGetKeyboardState(keyStates);
        }

        public static byte[] GetKeyboardState()
        {
            byte[] keyStates;

            if (!TryGetKeyboardState(out keyStates))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return keyStates;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool InsertMenu(
            IntPtr hmenu,
            uint position,
            MenuFlags flags,
            uint item_id,
            [MarshalAs(UnmanagedType.LPTStr)]string item_text);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool AppendMenu(IntPtr hMenu, MenuFlags uFlags, uint uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", EntryPoint = "VkKeyScanW", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short IntVkKeyScan(char key);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(VirtualKeys nVirtKey);

        [DllImport("user32.dll")]
        public static extern IntPtr GetMenu(IntPtr hWnd);

        /// <summary> 
        /// Function to retrieve raw input data. 
        /// </summary> 
        /// <param name="hRawInput">Handle to the raw input.</param> 
        /// <param name="uiCommand">Command to issue when retrieving data.</param> 
        /// <param name="pData">Raw input data.</param> 
        /// <param name="pcbSize">Number of bytes in the array.</param> 
        /// <param name="cbSizeHeader">Size of the header.</param> 
        /// <returns>0 if successful if pData is null, otherwise number of bytes if pData is not null.</returns> 
        [DllImport("user32.dll")]
        public static extern int GetRawInputData(
            IntPtr hRawInput,
            RawInputCommand uiCommand,
            out RAWINPUT pData,
            ref int pcbSize,
            int cbSizeHeader);

        [DllImport("user32")]
        public static extern bool ChangeWindowMessageFilter(
            uint msg,
            ChangeWindowMessageFilterFlags flags);

        /// <summary>Function to register a raw input device.</summary> 
        /// <param name="pRawInputDevices">Array of raw input devices.</param> 
        /// <param name="uiNumDevices">Number of devices.</param> 
        /// <param name="cbSize">Size of the RAWINPUTDEVICE structure.</param> 
        /// <returns>TRUE if successful, FALSE if not.</returns> 
        [DllImport("user32.dll")]
        public static extern bool RegisterRawInputDevices(
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] RAWINPUTDEVICE[] pRawInputDevices,
            int uiNumDevices,
            int cbSize); 

        /// <summary>
        /// A pointer to a POINT structure that receives the screen coordinates of the cursor.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms648390(v=vs.85).aspx
        /// </summary>
        /// <param name="lpPoint"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out Point lpPoint);

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms633522(v=vs.85).aspx
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        /// <summary>
        /// Retrieves a handle to the window that contains the specified point.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms633558(v=vs.85).aspx
        /// </summary>
        /// <param name="Point"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point Point);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        /// <summary>
        /// Retrieves a handle to the desktop window. The desktop window covers the entire screen. 
        /// The desktop window is the area on top of which other windows are painted. 
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms633504(v=vs.85).aspx
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// Retrieves a handle to the child window at the specified point. 
        /// The search is restricted to immediate child windows; grandchildren and deeper descendant windows are not searched.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms633537(v=vs.85).aspx
        /// </summary>
        /// <param name="hwndParent"></param>
        /// <param name="ptParentClientCoords"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr RealChildWindowFromPoint(IntPtr hwndParent, Point ptParentClientCoords);

        [DllImport("user32")]
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

        /// <summary>
        /// Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs..</param>
        /// <param name="nIndex">The zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer. To set any other value, specify one of the following values: GWL_EXSTYLE, GWL_HINSTANCE, GWL_ID, GWL_STYLE, GWL_USERDATA, GWL_WNDPROC </param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer. 
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError. </returns>
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, WindowStylesEx dwNewLong);

        [DllImport("user32.dll")]
        public static extern WindowStylesEx GetWindowLong(IntPtr hwnd, GWL index);

        /// <summary>
        /// The ClientToScreen function converts the client-area coordinates of a specified point to screen coordinates.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/dd183434(v=vs.85).aspx
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ClientToScreen(
            IntPtr hwnd,
            ref Point point);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpClassName">The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be in the low-order word of lpClassName; the high-order word must be zero. 
        /// If lpClassName points to a string, it specifies the window class name. The class name can be any name registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names. 
        /// If lpClassName is NULL, it finds any window whose title matches the lpWindowName parameter. 
        /// </param>
        /// <param name="lpWindowName">The window name (the window's title). If this parameter is NULL, all window names match. </param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        /// <summary>
        /// Sets the specified window's show state. 
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms633548(v=vs.85).aspx
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="showStyle"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle showStyle);

    }
}
