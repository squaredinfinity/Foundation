using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SquaredInfinity.Foundation.Win32Api
{
    public partial class dwmapi
    {
        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            DWMWINDOWATTRIBUTE dwAttributeToSet,
            IntPtr pvAttributeValue,
            uint cbAttribute);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetIconicThumbnail(
            IntPtr hwnd,
            IntPtr hbitmap,
            uint flags);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetIconicLivePreviewBitmap(
            IntPtr hwnd,
            IntPtr hbitmap,
            ref NativePoint ptClient,
            uint flags);

        [DllImport("dwmapi.dll")]
        public static extern int DwmInvalidateIconicBitmaps(IntPtr hwnd);
    }
}
