using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Extensions
{
    public static class Int32Extensions
    {
        static readonly Int32 BytesInMebibyte = 1048576;
        static readonly Int32 BytesInMegabyte = 1000000;

        public static object ToMegaBytes(this Int32 bytes)
        {
            if (bytes < BytesInMegabyte)
                return ((double)bytes) / BytesInMegabyte;

            return bytes / BytesInMegabyte;
        }

        public static object ToMebiBytes(this Int32 bytes)
        {
            if (bytes < BytesInMebibyte)
                return ((double)bytes) / BytesInMebibyte;

            return bytes / BytesInMebibyte;
        }

        public static bool IsFlagSet(this Int32 number, Int32 flag)
        {
            return (number & flag) == flag;
        }

        public static int Clamp(this Int32 number, Int32 inclusiveMin, Int32 inclusiveMax)
        {
            return Math.Max(inclusiveMin, Math.Min(number, inclusiveMax));
        }
    }
}
