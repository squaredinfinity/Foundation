using SquaredInfinity.Foundation.Types.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Concurrent;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using SquaredInfinity.Foundation.Types.Description;
using System.Threading;
using System.ComponentModel;

namespace SquaredInfinity.Foundation.Serialization
{
    public abstract partial class FlexiSerializer
    {
        protected class TypeSerializationStrategiesConcurrentDictionary
            : ConcurrentDictionary<Type, ITypeSerializationStrategy> { }
    }
}
