using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Types.Mapping
{
	public partial class TypeMapper
    {
        class TypeMappingStrategiesConcurrentDictionary 
            : ConcurrentDictionary<TypeMappingStrategyKey, ITypeMappingStrategy> { }

    }
}
