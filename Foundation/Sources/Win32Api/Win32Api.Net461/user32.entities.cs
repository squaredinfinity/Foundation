using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Win32Api
{
    public partial class user32
    {
        static class FlashWindow
        {
            /// <summary>
            /// Stop flashing. The system restores the window to its original stae.
            /// </summary>
            public const uint FLASHW_STOP = 0;

            /// <summary>
            /// Flash the window caption.
            /// </summary>
            public const uint FLASHW_CAPTION = 1;

            /// <summary>
            /// Flash the taskbar button.
            /// </summary>
            public const uint FLASHW_TRAY = 2;

            /// <summary>
            /// Flash both the window caption and taskbar button.
            /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
            /// </summary>
            public const uint FLASHW_ALL = 3;
            /// <summary>
            /// Flash continuously, until the FLASHW_STOP flag is set.
            /// </summary>
            public const uint FLASHW_TIMER = 4;
            /// <summary>
            /// Flash continuously until the window comes to the foreground.
            /// </summary>
            public const uint FLASHW_TIMERNOFG = 12;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            /// <summary>
            /// The size of the structure in bytes.
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
            /// </summary>
            public IntPtr hwnd;
            /// <summary>
            /// The Flash Status.
            /// </summary>
            public uint dwFlags;
            /// <summary>
            /// The number of times to Flash the window.
            /// </summary>
            public uint uCount;
            /// <summary>
            /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
            /// </summary>
            public uint dwTimeout;
        }

        [Flags]
        public enum MenuFlags : uint
        {
            MF_STRING = 0,
            MF_BYPOSITION = 0x400,
            MF_SEPARATOR = 0x800,
            MF_REMOVE = 0x1000,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MENUITEMINFO
        {
            public uint cbSize;
            public uint fMask;
            public uint fType;
            public uint fState;
            public uint wID;
            public IntPtr hSubMenu;
            public IntPtr hbmpChecked;
            public IntPtr hbmpUnchecked;
            public IntPtr dwItemData;
            public string dwTypeData;
            public uint cch;
            public IntPtr hbmpItem;

            // return the size of the structure
            public static uint sizeOf
            {
                get { return (uint)Marshal.SizeOf(typeof(MENUITEMINFO)); }
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct RawHID
        {
            short dwSizeHid;
            short dwCount;
            byte[] bRawData;
        }

        /// <summary> 
        /// Value type for a raw input header. 
        /// </summary> 
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTHEADER
        {
            /// <summary>Type of device the input is coming from.</summary> 
            public RawInputType Type;
            /// <summary>Size of the packet of data.</summary> 
            public int Size;
            /// <summary>Handle to the device sending the data.</summary> 
            public IntPtr Device;
            /// <summary>wParam from the window message.</summary> 
            public IntPtr wParam;
        }

        /// <summary> 
        /// Enumeration containing the flags for raw mouse data. 
        /// </summary> 
        [Flags()]
        public enum RawMouseFlags
            : ushort
        {
            /// <summary>Relative to the last position.</summary> 
            MoveRelative = 0,
            /// <summary>Absolute positioning.</summary> 
            MoveAbsolute = 1,
            /// <summary>Coordinate data is mapped to a virtual desktop.</summary> 
            VirtualDesktop = 2,
            /// <summary>Attributes for the mouse have changed.</summary> 
            AttributesChanged = 4
        }

        /// <summary> 
        /// Enumeration containing the button data for raw mouse input. 
        /// </summary> 
        [Flags()]
        public enum RawMouseButtons
            : ushort
        {
            /// <summary>No button.</summary> 
            None = 0,
            /// <summary>Left (button 1) down.</summary> 
            LeftDown = 0x0001,
            /// <summary>Left (button 1) up.</summary> 
            LeftUp = 0x0002,
            /// <summary>Right (button 2) down.</summary> 
            RightDown = 0x0004,
            /// <summary>Right (button 2) up.</summary> 
            RightUp = 0x0008,
            /// <summary>Middle (button 3) down.</summary> 
            MiddleDown = 0x0010,
            /// <summary>Middle (button 3) up.</summary> 
            MiddleUp = 0x0020,
            /// <summary>Button 4 down.</summary> 
            Button4Down = 0x0040,
            /// <summary>Button 4 up.</summary> 
            Button4Up = 0x0080,
            /// <summary>Button 5 down.</summary> 
            Button5Down = 0x0100,
            /// <summary>Button 5 up.</summary> 
            Button5Up = 0x0200,
            /// <summary>Mouse wheel moved.</summary> 
            MouseWheel = 0x0400
        }


        /// <summary> 
        /// Contains information about the state of the mouse. 
        /// </summary> 
        [StructLayout(LayoutKind.Explicit)]
        public struct RawMouse
        {
            /// <summary> 
            /// The mouse state. 
            /// </summary> 
            [FieldOffset(0)]
            public RawMouseFlags Flags;
            /// <summary> 
            /// Flags for the event. 
            /// </summary> 
            [FieldOffset(4)]
            public RawMouseButtons ButtonFlags;
            /// <summary> 
            /// If the mouse wheel is moved, this will contain the delta amount. 
            /// </summary> 
            [FieldOffset(6)]
            public ushort ButtonData;
            /// <summary> 
            /// Raw button data. 
            /// </summary> 
            [FieldOffset(8)]
            public uint RawButtons;
            /// <summary> 
            /// The motion in the X direction. This is signed relative motion or 
            /// absolute motion, depending on the value of usFlags. 
            /// </summary> 
            [FieldOffset(12)]
            public int LastX;
            /// <summary> 
            /// The motion in the Y direction. This is signed relative motion or absolute motion, 
            /// depending on the value of usFlags. 
            /// </summary> 
            [FieldOffset(16)]
            public int LastY;
            /// <summary> 
            /// The device-specific additional information for the event. 
            /// </summary> 
            [FieldOffset(20)]
            public uint ExtraInformation;
        }

        /// <summary> 
        /// Enumeration for virtual keys. 
        /// </summary> 
        public enum VirtualKeys
            : ushort
        {
            /// <summary></summary> 
            LeftButton = 0x01,
            /// <summary></summary> 
            RightButton = 0x02,
            /// <summary></summary> 
            Cancel = 0x03,
            /// <summary></summary> 
            MiddleButton = 0x04,
            /// <summary></summary> 
            ExtraButton1 = 0x05,
            /// <summary></summary> 
            ExtraButton2 = 0x06,
            /// <summary></summary> 
            Back = 0x08,
            /// <summary></summary> 
            Tab = 0x09,
            /// <summary></summary> 
            Clear = 0x0C,
            /// <summary></summary> 
            Return = 0x0D,
            /// <summary></summary> 
            Shift = 0x10,
            /// <summary></summary> 
            Control = 0x11,
            /// <summary></summary> 
            Menu = 0x12,
            /// <summary></summary> 
            Pause = 0x13,
            /// <summary></summary> 
            CapsLock = 0x14,
            /// <summary></summary> 
            Kana = 0x15,
            /// <summary></summary> 
            Hangeul = 0x15,
            /// <summary></summary> 
            Hangul = 0x15,
            /// <summary></summary> 
            Junja = 0x17,
            /// <summary></summary> 
            Final = 0x18,
            /// <summary></summary> 
            Hanja = 0x19,
            /// <summary></summary> 
            Kanji = 0x19,
            /// <summary></summary> 
            Escape = 0x1B,
            /// <summary></summary> 
            Convert = 0x1C,
            /// <summary></summary> 
            NonConvert = 0x1D,
            /// <summary></summary> 
            Accept = 0x1E,
            /// <summary></summary> 
            ModeChange = 0x1F,
            /// <summary></summary> 
            Space = 0x20,
            /// <summary></summary> 
            Prior = 0x21,
            /// <summary></summary> 
            Next = 0x22,
            /// <summary></summary> 
            End = 0x23,
            /// <summary></summary> 
            Home = 0x24,
            /// <summary></summary> 
            Left = 0x25,
            /// <summary></summary> 
            Up = 0x26,
            /// <summary></summary> 
            Right = 0x27,
            /// <summary></summary> 
            Down = 0x28,
            /// <summary></summary> 
            Select = 0x29,
            /// <summary></summary> 
            Print = 0x2A,
            /// <summary></summary> 
            Execute = 0x2B,
            /// <summary></summary> 
            Snapshot = 0x2C,
            /// <summary></summary> 
            Insert = 0x2D,
            /// <summary></summary> 
            Delete = 0x2E,
            /// <summary></summary> 
            Help = 0x2F,
            /// <summary></summary> 
            N0 = 0x30,
            /// <summary></summary> 
            N1 = 0x31,
            /// <summary></summary> 
            N2 = 0x32,
            /// <summary></summary> 
            N3 = 0x33,
            /// <summary></summary> 
            N4 = 0x34,
            /// <summary></summary> 
            N5 = 0x35,
            /// <summary></summary> 
            N6 = 0x36,
            /// <summary></summary> 
            N7 = 0x37,
            /// <summary></summary> 
            N8 = 0x38,
            /// <summary></summary> 
            N9 = 0x39,
            /// <summary></summary> 
            A = 0x41,
            /// <summary></summary> 
            B = 0x42,
            /// <summary></summary> 
            C = 0x43,
            /// <summary></summary> 
            D = 0x44,
            /// <summary></summary> 
            E = 0x45,
            /// <summary></summary> 
            F = 0x46,
            /// <summary></summary> 
            G = 0x47,
            /// <summary></summary> 
            H = 0x48,
            /// <summary></summary> 
            I = 0x49,
            /// <summary></summary> 
            J = 0x4A,
            /// <summary></summary> 
            K = 0x4B,
            /// <summary></summary> 
            L = 0x4C,
            /// <summary></summary> 
            M = 0x4D,
            /// <summary></summary> 
            N = 0x4E,
            /// <summary></summary> 
            O = 0x4F,
            /// <summary></summary> 
            P = 0x50,
            /// <summary></summary> 
            Q = 0x51,
            /// <summary></summary> 
            R = 0x52,
            /// <summary></summary> 
            S = 0x53,
            /// <summary></summary> 
            T = 0x54,
            /// <summary></summary> 
            U = 0x55,
            /// <summary></summary> 
            V = 0x56,
            /// <summary></summary> 
            W = 0x57,
            /// <summary></summary> 
            X = 0x58,
            /// <summary></summary> 
            Y = 0x59,
            /// <summary></summary> 
            Z = 0x5A,
            /// <summary></summary> 
            LeftWindows = 0x5B,
            /// <summary></summary> 
            RightWindows = 0x5C,
            /// <summary></summary> 
            Application = 0x5D,
            /// <summary></summary> 
            Sleep = 0x5F,
            /// <summary></summary> 
            Numpad0 = 0x60,
            /// <summary></summary> 
            Numpad1 = 0x61,
            /// <summary></summary> 
            Numpad2 = 0x62,
            /// <summary></summary> 
            Numpad3 = 0x63,
            /// <summary></summary> 
            Numpad4 = 0x64,
            /// <summary></summary> 
            Numpad5 = 0x65,
            /// <summary></summary> 
            Numpad6 = 0x66,
            /// <summary></summary> 
            Numpad7 = 0x67,
            /// <summary></summary> 
            Numpad8 = 0x68,
            /// <summary></summary> 
            Numpad9 = 0x69,
            /// <summary></summary> 
            Multiply = 0x6A,
            /// <summary></summary> 
            Add = 0x6B,
            /// <summary></summary> 
            Separator = 0x6C,
            /// <summary></summary> 
            Subtract = 0x6D,
            /// <summary></summary> 
            Decimal = 0x6E,
            /// <summary></summary> 
            Divide = 0x6F,
            /// <summary></summary> 
            F1 = 0x70,
            /// <summary></summary> 
            F2 = 0x71,
            /// <summary></summary> 
            F3 = 0x72,
            /// <summary></summary> 
            F4 = 0x73,
            /// <summary></summary> 
            F5 = 0x74,
            /// <summary></summary> 
            F6 = 0x75,
            /// <summary></summary> 
            F7 = 0x76,
            /// <summary></summary> 
            F8 = 0x77,
            /// <summary></summary> 
            F9 = 0x78,
            /// <summary></summary> 
            F10 = 0x79,
            /// <summary></summary> 
            F11 = 0x7A,
            /// <summary></summary> 
            F12 = 0x7B,
            /// <summary></summary> 
            F13 = 0x7C,
            /// <summary></summary> 
            F14 = 0x7D,
            /// <summary></summary> 
            F15 = 0x7E,
            /// <summary></summary> 
            F16 = 0x7F,
            /// <summary></summary> 
            F17 = 0x80,
            /// <summary></summary> 
            F18 = 0x81,
            /// <summary></summary> 
            F19 = 0x82,
            /// <summary></summary> 
            F20 = 0x83,
            /// <summary></summary> 
            F21 = 0x84,
            /// <summary></summary> 
            F22 = 0x85,
            /// <summary></summary> 
            F23 = 0x86,
            /// <summary></summary> 
            F24 = 0x87,
            /// <summary></summary> 
            NumLock = 0x90,
            /// <summary></summary> 
            ScrollLock = 0x91,
            /// <summary></summary> 
            NEC_Equal = 0x92,
            /// <summary></summary> 
            Fujitsu_Jisho = 0x92,
            /// <summary></summary> 
            Fujitsu_Masshou = 0x93,
            /// <summary></summary> 
            Fujitsu_Touroku = 0x94,
            /// <summary></summary> 
            Fujitsu_Loya = 0x95,
            /// <summary></summary> 
            Fujitsu_Roya = 0x96,
            /// <summary></summary> 
            LeftShift = 0xA0,
            /// <summary></summary> 
            RightShift = 0xA1,
            /// <summary></summary> 
            LeftControl = 0xA2,
            /// <summary></summary> 
            RightControl = 0xA3,
            /// <summary></summary> 
            LeftMenu = 0xA4,
            /// <summary></summary> 
            RightMenu = 0xA5,
            /// <summary></summary> 
            BrowserBack = 0xA6,
            /// <summary></summary> 
            BrowserForward = 0xA7,
            /// <summary></summary> 
            BrowserRefresh = 0xA8,
            /// <summary></summary> 
            BrowserStop = 0xA9,
            /// <summary></summary> 
            BrowserSearch = 0xAA,
            /// <summary></summary> 
            BrowserFavorites = 0xAB,
            /// <summary></summary> 
            BrowserHome = 0xAC,
            /// <summary></summary> 
            VolumeMute = 0xAD,
            /// <summary></summary> 
            VolumeDown = 0xAE,
            /// <summary></summary> 
            VolumeUp = 0xAF,
            /// <summary></summary> 
            MediaNextTrack = 0xB0,
            /// <summary></summary> 
            MediaPrevTrack = 0xB1,
            /// <summary></summary> 
            MediaStop = 0xB2,
            /// <summary></summary> 
            MediaPlayPause = 0xB3,
            /// <summary></summary> 
            LaunchMail = 0xB4,
            /// <summary></summary> 
            LaunchMediaSelect = 0xB5,
            /// <summary></summary> 
            LaunchApplication1 = 0xB6,
            /// <summary></summary> 
            LaunchApplication2 = 0xB7,
            /// <summary></summary> 
            OEM1 = 0xBA,
            /// <summary></summary> 
            OEMPlus = 0xBB,
            /// <summary></summary> 
            OEMComma = 0xBC,
            /// <summary></summary> 
            OEMMinus = 0xBD,
            /// <summary></summary> 
            OEMPeriod = 0xBE,
            /// <summary></summary> 
            OEM2 = 0xBF,
            /// <summary></summary> 
            OEM3 = 0xC0,
            /// <summary></summary> 
            OEM4 = 0xDB,
            /// <summary></summary> 
            OEM5 = 0xDC,
            /// <summary></summary> 
            OEM6 = 0xDD,
            /// <summary></summary> 
            OEM7 = 0xDE,
            /// <summary></summary> 
            OEM8 = 0xDF,
            /// <summary></summary> 
            OEMAX = 0xE1,
            /// <summary></summary> 
            OEM102 = 0xE2,
            /// <summary></summary> 
            ICOHelp = 0xE3,
            /// <summary></summary> 
            ICO00 = 0xE4,
            /// <summary></summary> 
            ProcessKey = 0xE5,
            /// <summary></summary> 
            ICOClear = 0xE6,
            /// <summary></summary> 
            Packet = 0xE7,
            /// <summary></summary> 
            OEMReset = 0xE9,
            /// <summary></summary> 
            OEMJump = 0xEA,
            /// <summary></summary> 
            OEMPA1 = 0xEB,
            /// <summary></summary> 
            OEMPA2 = 0xEC,
            /// <summary></summary> 
            OEMPA3 = 0xED,
            /// <summary></summary> 
            OEMWSCtrl = 0xEE,
            /// <summary></summary> 
            OEMCUSel = 0xEF,
            /// <summary></summary> 
            OEMATTN = 0xF0,
            /// <summary></summary> 
            OEMFinish = 0xF1,
            /// <summary></summary> 
            OEMCopy = 0xF2,
            /// <summary></summary> 
            OEMAuto = 0xF3,
            /// <summary></summary> 
            OEMENLW = 0xF4,
            /// <summary></summary> 
            OEMBackTab = 0xF5,
            /// <summary></summary> 
            ATTN = 0xF6,
            /// <summary></summary> 
            CRSel = 0xF7,
            /// <summary></summary> 
            EXSel = 0xF8,
            /// <summary></summary> 
            EREOF = 0xF9,
            /// <summary></summary> 
            Play = 0xFA,
            /// <summary></summary> 
            Zoom = 0xFB,
            /// <summary></summary> 
            Noname = 0xFC,
            /// <summary></summary> 
            PA1 = 0xFD,
            /// <summary></summary> 
            OEMClear = 0xFE
        }


        /// <summary> 
        /// Enumeration containing flags for raw keyboard input. 
        /// </summary> 
        [Flags]
        public enum RawKeyboardFlags : ushort
        {
            /// <summary></summary> 
            KeyMake = 0,
            /// <summary></summary> 
            KeyBreak = 1,
            /// <summary></summary> 
            KeyE0 = 2,
            /// <summary></summary> 
            KeyE1 = 4,
            /// <summary></summary> 
            TerminalServerSetLED = 8,
            /// <summary></summary> 
            TerminalServerShadow = 0x10,
            /// <summary></summary> 
            TerminalServerVKPACKET = 0x20
        }


        /// <summary> 
        /// Value type for raw input from a keyboard. 
        /// </summary>     
        [StructLayout(LayoutKind.Sequential)]
        public struct RawKeyboard
        {
            /// <summary>Scan code for key depression.</summary> 
            public short MakeCode;
            /// <summary>Scan code information.</summary> 
            public RawKeyboardFlags Flags;
            /// <summary>Reserved.</summary> 
            public short Reserved;
            /// <summary>Virtual key code.</summary> 
            public VirtualKeys VirtualKey;
            /// <summary>Corresponding window message.</summary> 
            public int Message;
            /// <summary>Extra information.</summary> 
            public int ExtraInformation;
        }

        /// <summary> 
        /// Value type for raw input. 
        /// </summary> 
        [StructLayout(LayoutKind.Explicit)]
        public struct RAWINPUT
        {
            /// <summary>Header for the data.</summary> 
            [FieldOffset(0)]
            public RAWINPUTHEADER Header;
            /// <summary>Mouse raw input data.</summary> 
            [FieldOffset(16)]
            public RawMouse Mouse;
            ///// <summary>Keyboard raw input data.</summary> 
            //[FieldOffset(16)] 
            //public RawKeyboard Keyboard; 
            ///// <summary>HID raw input data.</summary> 
            //[FieldOffset(16)] 
            //public RawHID Hid; 
        }


        /// <summary> 
        /// Enumeration contanining the command types to issue. 
        /// </summary> 
        public enum RawInputCommand
        {
            /// <summary> 
            /// Get input data. 
            /// </summary> 
            Input = 0x10000003,
            /// <summary> 
            /// Get header data. 
            /// </summary> 
            Header = 0x10000005
        }

        /// <summary> 
        /// Enumeration containing the type device the raw input is coming from. 
        /// </summary> 
        public enum RawInputType
        {
            /// <summary> 
            /// Mouse input. 
            /// </summary> 
            Mouse = 0,
            /// <summary> 
            /// Keyboard input. 
            /// </summary> 
            Keyboard = 1,
            /// <summary> 
            /// Another device that is not the keyboard or the mouse. 
            /// </summary> 
            HID = 2
        }

        /// <summary>Value type for raw input devices.</summary> 
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICE
        {
            /// <summary>Top level collection Usage page for the raw input device.</summary> 
            public short UsagePage;
            /// <summary>Top level collection Usage for the raw input device. </summary> 
            public short Usage;
            /// <summary>Mode flag that specifies how to interpret the information provided by UsagePage and Usage.</summary> 
            public RawInputDeviceFlags Flags;
            /// <summary>Handle to the target device. If NULL, it follows the keyboard focus.</summary> 
            public IntPtr WindowHandle;
        }

        /// <summary>Enumeration containing flags for a raw input device.</summary> 
        [Flags()]
        public enum RawInputDeviceFlags
        {
            /// <summary>No flags.</summary> 
            None = 0,
            /// <summary>If set, this removes the top level collection from the inclusion list. This tells the operating system to stop reading from a device which matches the top level collection.</summary> 
            Remove = 0x00000001,
            /// <summary>If set, this specifies the top level collections to exclude when reading a complete usage page. This flag only affects a TLC whose usage page is already specified with PageOnly.</summary> 
            Exclude = 0x00000010,
            /// <summary>If set, this specifies all devices whose top level collection is from the specified usUsagePage. Note that Usage must be zero. To exclude a particular top level collection, use Exclude.</summary> 
            PageOnly = 0x00000020,
            /// <summary>If set, this prevents any devices specified by UsagePage or Usage from generating legacy messages. This is only for the mouse and keyboard.</summary> 
            NoLegacy = 0x00000030,
            /// <summary>If set, this enables the caller to receive the input even when the caller is not in the foreground. Note that WindowHandle must be specified.</summary> 
            InputSink = 0x00000100,
            /// <summary>If set, the mouse button click does not activate the other window.</summary> 
            CaptureMouse = 0x00000200,
            /// <summary>If set, the application-defined keyboard device hotkeys are not handled. However, the system hotkeys; for example, ALT+TAB and CTRL+ALT+DEL, are still handled. By default, all keyboard hotkeys are handled. NoHotKeys can be specified even if NoLegacy is not specified and WindowHandle is NULL.</summary> 
            NoHotKeys = 0x00000200,
            /// <summary>If set, application keys are handled.  NoLegacy must be specified.  Keyboard only.</summary> 
            AppKeys = 0x00000400
        }





        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public enum GetWindowCommandd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [Flags]
        public enum MouseEventFlags : uint
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800,
            XDOWN = 0x00000080,
            XUP = 0x00000100
        }

        //Use the values of this enum for the 'dwData' parameter
        //to specify an X button when using MouseEventFlags.XDOWN or
        //MouseEventFlags.XUP for the dwFlags parameter.
        public enum MouseEventDataXButtons : uint
        {
            XBUTTON1 = 0x00000001,
            XBUTTON2 = 0x00000002
        }

        [Flags]
        public enum WindowStylesEx : uint
        {
            /// <summary>The window has a thin-line border.</summary>
            WS_BORDER = 0x800000,

            /// <summary>The window has a title bar (includes the WS_BORDER style).</summary>
            WS_CAPTION = 0xc00000,

            /// <summary>The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.</summary>
            WS_CHILD = 0x40000000,

            /// <summary>Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.</summary>
            WS_CLIPCHILDREN = 0x2000000,

            /// <summary>
            /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
            /// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
            /// </summary>
            WS_CLIPSIBLINGS = 0x4000000,

            /// <summary>The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.</summary>
            WS_DISABLED = 0x8000000,

            /// <summary>The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.</summary>
            WS_DLGFRAME = 0x400000,

            /// <summary>
            /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style.
            /// The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
            /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
            /// </summary>
            WS_GROUP = 0x20000,

            /// <summary>The window has a horizontal scroll bar.</summary>
            WS_HSCROLL = 0x100000,

            /// <summary>The window is initially maximized.</summary> 
            WS_MAXIMIZE = 0x1000000,

            /// <summary>The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary> 
            WS_MAXIMIZEBOX = 0x10000,

            /// <summary>The window is initially minimized.</summary>
            WS_MINIMIZE = 0x20000000,

            /// <summary>The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
            WS_MINIMIZEBOX = 0x20000,

            /// <summary>The window is an overlapped window. An overlapped window has a title bar and a border.</summary>
            WS_OVERLAPPED = 0x0,

            /// <summary>The window is an overlapped window.</summary>
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_SIZEFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

            /// <summary>The window is a pop-up window. This style cannot be used with the WS_CHILD style.</summary>
            WS_POPUP = 0x80000000u,

            /// <summary>The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.</summary>
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

            /// <summary>The window has a sizing border.</summary>
            WS_SIZEFRAME = 0x40000,

            /// <summary>The window has a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
            WS_SYSMENU = 0x80000,

            /// <summary>
            /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
            /// Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.  
            /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
            /// For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
            /// </summary>
            WS_TABSTOP = 0x10000,

            /// <summary>The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.</summary>
            WS_VISIBLE = 0x10000000,

            /// <summary>The window has a vertical scroll bar.</summary>
            WS_VSCROLL = 0x200000,

            /// <summary>
            /// Specifies that a window created with this style accepts drag-drop files.
            /// </summary>
            WS_EX_ACCEPTFILES = 0x00000010,
            /// <summary>
            /// Forces a top-level window onto the taskbar when the window is visible.
            /// </summary>
            WS_EX_APPWINDOW = 0x00040000,
            /// <summary>
            /// Specifies that a window has a border with a sunken edge.
            /// </summary>
            WS_EX_CLIENTEDGE = 0x00000200,
            /// <summary>
            /// Windows XP: Paints all descendants of a window in bottom-to-top painting order using double-buffering. For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
            /// </summary>
            WS_EX_COMPOSITED = 0x02000000,
            /// <summary>
            /// Includes a question mark in the title bar of the window. When the user clicks the question mark, the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message. The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command. The Help application displays a pop-up window that typically contains help for the child window.
            /// WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
            /// </summary>
            WS_EX_CONTEXTHELP = 0x00000400,
            /// <summary>
            /// The window itself contains child windows that should take part in dialog box navigation. If this style is specified, the dialog manager recurses into children of this window when performing navigation operations such as handling the TAB key, an arrow key, or a keyboard mnemonic.
            /// </summary>
            WS_EX_CONTROLPARENT = 0x00010000,
            /// <summary>
            /// Creates a window that has a double border; the window can, optionally, be created with a title bar by specifying the WS_CAPTION style in the dwStyle parameter.
            /// </summary>
            WS_EX_DLGMODALFRAME = 0x00000001,
            /// <summary>
            /// Windows 2000/XP: Creates a layered window. Note that this cannot be used for child windows. Also, this cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
            /// </summary>
            WS_EX_LAYERED = 0x00080000,
            /// <summary>
            /// Arabic and Hebrew versions of Windows 98/Me, Windows 2000/XP: Creates a window whose horizontal origin is on the right edge. Increasing horizontal values advance to the left. 
            /// </summary>
            WS_EX_LAYOUTRTL = 0x00400000,
            /// <summary>
            /// Creates a window that has generic left-aligned properties. This is the default.
            /// </summary>
            WS_EX_LEFT = 0x00000000,
            /// <summary>
            /// If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the vertical scroll bar (if present) is to the left of the client area. For other languages, the style is ignored.
            /// </summary>
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            /// <summary>
            /// The window text is displayed using left-to-right reading-order properties. This is the default.
            /// </summary>
            WS_EX_LTRREADING = 0x00000000,
            /// <summary>
            /// Creates a multiple-document interface (MDI) child window.
            /// </summary>
            WS_EX_MDICHILD = 0x00000040,
            /// <summary>
            /// Windows 2000/XP: A top-level window created with this style does not become the foreground window when the user clicks it. The system does not bring this window to the foreground when the user minimizes or closes the foreground window. 
            /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
            /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the WS_EX_APPWINDOW style.
            /// </summary>
            WS_EX_NOACTIVATE = 0x08000000,
            /// <summary>
            /// Windows 2000/XP: A window created with this style does not pass its window layout to its child windows.
            /// </summary>
            WS_EX_NOINHERITLAYOUT = 0x00100000,
            /// <summary>
            /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
            /// </summary>
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            /// <summary>
            /// Combines the WS_EX_CLIENTEDGE and WS_EX_WINDOWEDGE styles.
            /// </summary>
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            /// <summary>
            /// Combines the WS_EX_WINDOWEDGE, WS_EX_TOOLWINDOW, and WS_EX_TOPMOST styles.
            /// </summary>
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
            /// <summary>
            /// The window has generic "right-aligned" properties. This depends on the window class. This style has an effect only if the shell language is Hebrew, Arabic, or another language that supports reading-order alignment; otherwise, the style is ignored.
            /// Using the WS_EX_RIGHT style for static or edit controls has the same effect as using the SS_RIGHT or ES_RIGHT style, respectively. Using this style with button controls has the same effect as using BS_RIGHT and BS_RIGHTBUTTON styles.
            /// </summary>
            WS_EX_RIGHT = 0x00001000,
            /// <summary>
            /// Vertical scroll bar (if present) is to the right of the client area. This is the default.
            /// </summary>
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            /// <summary>
            /// If the shell language is Hebrew, Arabic, or another language that supports reading-order alignment, the window text is displayed using right-to-left reading-order properties. For other languages, the style is ignored.
            /// </summary>
            WS_EX_RTLREADING = 0x00002000,
            /// <summary>
            /// Creates a window with a three-dimensional border style intended to be used for items that do not accept user input.
            /// </summary>
            WS_EX_STATICEDGE = 0x00020000,
            /// <summary>
            /// Creates a tool window; that is, a window intended to be used as a floating toolbar. A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font. A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB. If a tool window has a system menu, its icon is not displayed on the title bar. However, you can display the system menu by right-clicking or by typing ALT+SPACE. 
            /// </summary>
            WS_EX_TOOLWINDOW = 0x00000080,
            /// <summary>
            /// Specifies that a window created with this style should be placed above all non-topmost windows and should stay above them, even when the window is deactivated. To add or remove this style, use the SetWindowPos function.
            /// </summary>
            WS_EX_TOPMOST = 0x00000008,
            /// <summary>
            /// Specifies that a window created with this style should not be painted until siblings beneath the window (that were created by the same thread) have been painted. The window appears transparent because the bits of underlying sibling windows have already been painted.
            /// To achieve transparency without these restrictions, use the SetWindowRgn function.
            /// </summary>
            WS_EX_TRANSPARENT = 0x00000020,
            /// <summary>
            /// Specifies that a window has a border with a raised edge.
            /// </summary>
            WS_EX_WINDOWEDGE = 0x00000100
        }

        public enum GWL : int
        {
            /// <summary>
            /// Sets a new extended window style.
            /// </summary>
            GWL_EXSTYLE = -20,

            /// <summary>
            /// Sets a new application instance handle.
            /// </summary>
            GWL_HINSTANCE = -6,

            /// <summary>
            /// Sets a new identifier of the child window. The window cannot be a top-level window.
            /// </summary>
            GWL_ID = -12,

            /// <summary>
            /// Sets a new window style.
            /// </summary>
            GWL_STYLE = -16,

            /// <summary>
            /// Sets the user data associated with the window. This data is intended for use by the application that created the window. Its value is initially zero.
            /// </summary
            GWL_USERDATA = -21,

            /// <summary>
            /// Sets a new address 
            /// </summary>
            GWL_WNDPROC = -4
        }

        public enum ChangeWindowMessageFilterFlags : uint
        {
            MSGFLT_ADD = 1,
            MSGFLT_REMOVE = 2
        }

        /// <summary>Enumeration of the different ways of showing a window using 
        /// ShowWindow</summary>
        public enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window is minimized 
            /// or maximized, the system restores it to its original size and 
            /// position. An application should specify this flag when displaying 
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position. 
            /// This value is similar to "ShowNormal", except the window is not 
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size 
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next 
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is 
            /// similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This 
            /// value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is 
            /// minimized or maximized, the system restores it to its original size 
            /// and position. An application should specify this flag when restoring 
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified in the 
            /// STARTUPINFO structure passed to the CreateProcess function by the 
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread 
            /// that owns the window is hung. This flag should only be used when 
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        public enum SetWindowPosFlags : uint
        {
            /// <summary>
            /// If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request. 
            /// </summary>
            SWP_ASYNCWINDOWPOS = 0x4000,
            /// <summary>
            /// Prevents generation of the WM_SYNCPAINT message. 
            /// </summary>
            SWP_DEFERERASE = 0x2000,
            /// <summary>
            /// Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            SWP_DRAWFRAME = 0x0020,
            /// <summary>
            /// Applies new frame styles set using the SetWindowLong function.Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            /// </summary>
            SWP_FRAMECHANGED = 0x0020,
            /// <summary>
            /// Hides the window.
            /// </summary>
            SWP_HIDEWINDOW = 0x0080,
            /// <summary>
            /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOACTIVATE = 0x0010,
            /// <summary>
            /// Discards the entire contents of the client area.If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            /// </summary>
            SWP_NOCOPYBITS = 0x0100,
            /// <summary>
            /// Retains the current position (ignores X and Y parameters).
            /// </summary>
            SWP_NOMOVE = 0x0002,
            /// <summary>
            /// Does not change the owner window's position in the Z order.
            /// </summary>
            SWP_NOOWNERZORDER = 0x0200,
            /// <summary>
            /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            SWP_NOREDRAW = 0x0008,
            /// <summary>
            /// Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            SWP_NOREPOSITION = 0x0200,
            /// <summary>
            /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            SWP_NOSENDCHANGING = 0x0400,
            /// <summary>
            /// Retains the current size(ignores the cx and cy parameters).
            /// </summary>
            SWP_NOSIZE = 0x0001,
            /// <summary>
            /// Retains the current Z order(ignores the hWndInsertAfter parameter).
            /// </summary>
            SWP_NOZORDER = 0x0004,
            /// <summary>
            /// Displays the window
            /// </summary>
            SWP_SHOWWINDOW = 0x0040
        }

    }
}
