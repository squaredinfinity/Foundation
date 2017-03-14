using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Serialization.FlexiPlainText
{
    public interface IFlexiPlainTextSerializationStrategy : ITypeSerializationStrategy
    {
        string Serialize(
            object instance,
            IFlexiPlainTextSerializationContext cx,
            out bool hasAlreadyBeenSerialized);

        string GetNameFromType(Type type);
    }
}
