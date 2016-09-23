using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class LongExtensions
    {
        static readonly long BytesInMebibyte = 1048576;
        static readonly long BytesInMegabyte = 1000000;

        public static object ToMegaBytes(this long bytes)
        {
            if (bytes < BytesInMegabyte)
                return ((double)bytes) / BytesInMegabyte;

            return bytes / BytesInMegabyte;
        }

        public static object ToMebiBytes(this long bytes)
        {
            if (bytes < BytesInMebibyte)
                return ((double)bytes) / BytesInMebibyte;

            return bytes / BytesInMebibyte;
        }
    }
}
