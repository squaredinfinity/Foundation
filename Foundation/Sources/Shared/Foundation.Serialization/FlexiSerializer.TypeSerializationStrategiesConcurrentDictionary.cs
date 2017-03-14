using SquaredInfinity.Types.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SquaredInfinity.Extensions;
using System.Collections.Concurrent;
using SquaredInfinity.Types.Description.Reflection;
using SquaredInfinity.Types.Description;
using System.Threading;
using System.ComponentModel;

namespace SquaredInfinity.Serialization
{
    public abstract partial class FlexiSerializer
    {
        protected class TypeSerializationStrategiesConcurrentDictionary
            : ConcurrentDictionary<Type, ITypeSerializationStrategy> { }
    }
}
