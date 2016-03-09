using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Serialization.FlexiPlainText
{
    public interface IFlexiPlainTextSerializationContext : ISerializationContext
    {
        string Serialize(object obj);
    }
}
