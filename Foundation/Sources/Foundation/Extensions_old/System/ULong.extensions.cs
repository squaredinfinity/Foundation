using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Extensions
{
    public static class ULongExtensions
    {
        static readonly ulong BytesInMebibyte = 1048576;
        static readonly ulong BytesInMegabyte = 1000000;

        public static object ToMegaBytes(this ulong bytes)
        {
            if (bytes < BytesInMegabyte)
                return ((double)bytes) / BytesInMegabyte;

            return bytes / BytesInMegabyte;
        }

        public static object ToMebiBytes(this ulong bytes)
        {
            if (bytes < BytesInMebibyte)
                return ((double)bytes) / BytesInMebibyte;

            return bytes / BytesInMebibyte;
        }
    }
}
