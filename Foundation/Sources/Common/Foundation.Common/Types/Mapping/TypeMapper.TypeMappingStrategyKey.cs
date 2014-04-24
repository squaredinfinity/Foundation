using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Types.Mapping
{
	public partial class TypeMapper
    {
        struct TypeMappingStrategyKey : IEquatable<TypeMappingStrategyKey>
        {
            public Type SourceType;
            public Type TargetType;

            public TypeMappingStrategyKey(Type source, Type target)
            {
                this.SourceType = source;
                this.TargetType = target;
            }

            public bool Equals(TypeMappingStrategyKey other)
            {
                return
                    SourceType == other.SourceType
                    && TargetType == other.TargetType;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as TypeMappingStrategy);
            }

            public override int GetHashCode()
            {
                var hash = 21;

                hash *= 31 ^ SourceType.GetHashCode();
                hash *= 31 ^ TargetType.GetHashCode();

                return hash;
            }
        }

    }
}
