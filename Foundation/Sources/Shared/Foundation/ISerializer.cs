using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation
{
    public interface ISerialzier
    {
        SerializedDataInfo Serialize<T>(T obj);
        T Deserialize<T>(SerializedDataInfo data);
    }
}
