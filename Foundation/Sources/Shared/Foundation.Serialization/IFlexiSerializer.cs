using SquaredInfinity.Serialization.FlexiXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Serialization
{
    public interface IFlexiSerializer
    {
        ITypeSerializationStrategy GetTypeSerializationStrategy(Type type);
        ITypeSerializationStrategy GetOrCreateTypeSerializationStrategy(Type type);
    }
}
