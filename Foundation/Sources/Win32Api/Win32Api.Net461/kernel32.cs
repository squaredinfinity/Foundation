using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SquaredInfinity.Win32Api
{
    public static partial class kernel32
    {
        static readonly UInt32 StdOutputHandle = 0xFFFFFFF5;

        /// <summary>
        /// Opens a console and redirects StdOut stream to it.
        /// </summary>
        public static void OpenConsole()
        {
            AllocConsole();

            // stdout's handle is always 7
            IntPtr defaultStdout = new IntPtr(7);
            IntPtr currentStdout = GetStdHandle(StdOutputHandle);

            if (currentStdout != defaultStdout)
            {
                // reset stdout
                SetStdHandle(StdOutputHandle, defaultStdout);
            }

            // reopen stdout
            TextWriter writer = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };

            Console.SetOut(writer);
        }
    }
}
