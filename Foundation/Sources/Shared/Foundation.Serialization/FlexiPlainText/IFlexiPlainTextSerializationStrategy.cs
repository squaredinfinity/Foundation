using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Serialization.FlexiPlainText
{
    public interface IFlexiPlainTextSerializationStrategy : ITypeSerializationStrategy
    {
        string Serialize(
            object instance,
            IFlexiPlainTextSerializationContext cx,
            out bool hasAlreadyBeenSerialized);        
    }
}
