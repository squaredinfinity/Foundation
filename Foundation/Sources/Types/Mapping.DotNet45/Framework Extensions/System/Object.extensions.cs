using SquaredInfinity.Types.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class ObjectExtensions
    {
        static readonly TypeMapper DefaultTypeMapper = new TypeMapper();

        public static void DeepCopyFrom<TTarget>(this TTarget target, object source)
        {
            target.DeepCopyFrom<TTarget>(source, MappingOptions.DefaultCopy);
        }

        public static void DeepCopyFrom<TTarget>(this TTarget target, object source, MappingOptions options)
        {
            DefaultTypeMapper.Map(source, target, options);
        }

        public static TTarget DeepClone<TTarget>(this TTarget source)
        {
            var targetType = typeof(TTarget);

            if (targetType.IsAbstract || targetType.IsInterface)
            {
                if (source != null && source is TTarget)
                {
                    return (TTarget)DefaultTypeMapper.DeepClone(source, source.GetType());
                }
                else
                {
                    throw new ArgumentException("Unable to clone to interface or abstract class.");
                }
            }
            else
            {
                return DefaultTypeMapper.DeepClone(source);
            }
        }

        public static object DeepClone(this object source, Type sourceType)
        {
            return DefaultTypeMapper.DeepClone(source, sourceType);
        }
    }
}
