using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity
{
    public interface ISerializer
    {
        SerializedDataInfo Serialize<T>(T obj);
        T Deserialize<T>(SerializedDataInfo data);
    }
}
