using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SquaredInfinity.Win32Api
{
    public partial class dwmapi
    {
        public enum DWMWINDOWATTRIBUTE : uint
        {
            /// <summary>
            /// [get] Is non-client rendering enabled/disabled
            /// </summary>
            DWMWA_NCRENDERING_ENABLED = 1,       
            /// <summary>
            /// [set] Non-client rendering policy
            /// </summary>
            DWMWA_NCRENDERING_POLICY,
            /// <summary>
            /// [set] Potentially enable/forcibly disable transitions
            /// </summary>
            DWMWA_TRANSITIONS_FORCEDISABLED,   
            /// <summary>
            /// [set] Allow contents rendered in the non-client area to be visible on the DWM-drawn frame.
            /// </summary>
            DWMWA_ALLOW_NCPAINT,
            /// <summary>
            /// [get] Bounds of the caption button area in window-relative space.
            /// </summary>
            DWMWA_CAPTION_BUTTON_BOUNDS,
            /// <summary>
            /// [set] Is non-client content RTL mirrored
            /// </summary>
            DWMWA_NONCLIENT_RTL_LAYOUT,
            /// <summary>
            /// [set] Force this window to display iconic thumbnails.
            /// </summary>
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            /// <summary>
            /// [set] Designates how Flip3D will treat the window.
            /// </summary>
            DWMWA_FLIP3D_POLICY,
            /// <summary>
            /// [get] Gets the extended frame bounds rectangle in screen space
            /// </summary>
            DWMWA_EXTENDED_FRAME_BOUNDS,
            /// <summary>
            /// [set] Indicates an available bitmap when there is no better thumbnail representation.
            /// </summary>
            DWMWA_HAS_ICONIC_BITMAP,
            /// <summary>
            ///  [set] Don't invoke Peek on the window.
            /// </summary>
            DWMWA_DISALLOW_PEEK,
            /// <summary>
            /// [set] LivePreview exclusion information
            /// </summary>
            DWMWA_EXCLUDED_FROM_PEEK,      
            /// <summary>
            /// ???
            /// </summary>                             
            DWMWA_LAST
        };

        public enum WM_Messages : int
        {
            WM_DWMCOMPOSITIONCHANGED = 0x031E,
            WM_DWMNCRENDERINGCHANGED = 0x031F,
            WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320,
            WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321,
            WM_DWMSENDICONICTHUMBNAIL = 0x0323,
            WM_DWMSENDICONICLIVEPREVIEWBITMAP = 0x0326
        }

        /// <summary>
        /// A wrapper for the native POINT structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NativePoint
        {
            /// <summary>
            /// Initialize the NativePoint
            /// </summary>
            /// <param name="x">The x coordinate of the point.</param>
            /// <param name="y">The y coordinate of the point.</param>
            public NativePoint(int x, int y)
                : this()
            {
                X = x;
                Y = y;
            }

            /// <summary>
            /// The X coordinate of the point
            /// </summary>        
            public int X { get; set; }

            /// <summary>
            /// The Y coordinate of the point
            /// </summary>                                
            public int Y { get; set; }

            /// <summary>
            /// Determines if two NativePoints are equal.
            /// </summary>
            /// <param name="first">First NativePoint</param>
            /// <param name="second">Second NativePoint</param>
            /// <returns>True if first NativePoint is equal to the second; false otherwise.</returns>
            public static bool operator ==(NativePoint first, NativePoint second)
            {
                return first.X == second.X
                    && first.Y == second.Y;
            }

            /// <summary>
            /// Determines if two NativePoints are not equal.
            /// </summary>
            /// <param name="first">First NativePoint</param>
            /// <param name="second">Second NativePoint</param>
            /// <returns>True if first NativePoint is not equal to the second; false otherwise.</returns>
            public static bool operator !=(NativePoint first, NativePoint second)
            {
                return !(first == second);
            }

            /// <summary>
            /// Determines if this NativePoint is equal to another.
            /// </summary>
            /// <param name="obj">Another NativePoint to compare</param>
            /// <returns>True if this NativePoint is equal obj; false otherwise.</returns>
            public override bool Equals(object obj)
            {
                return (obj != null && obj is NativePoint) ? this == (NativePoint)obj : false;
            }

            /// <summary>
            /// Gets a hash code for the NativePoint.
            /// </summary>
            /// <returns>Hash code for the NativePoint</returns>
            public override int GetHashCode()
            {
                int hash = X.GetHashCode();
                hash = hash * 31 + Y.GetHashCode();
                return hash;
            }
        }
    }
}
