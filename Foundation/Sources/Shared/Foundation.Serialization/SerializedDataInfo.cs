using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity
{
    public class SerializedDataInfo
    {
        public readonly byte[] Bytes;
        public readonly Encoding Encoding;

        public SerializedDataInfo(byte[] bytes, Encoding encoding)
        {
            this.Bytes = bytes;
            this.Encoding = encoding;
        }
    }
}
