using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Serialization.FlexiPlainText
{
    public interface IFlexiPlainTextSerializationContext : ISerializationContext
    {
        string Serialize(object obj);
    }
}
