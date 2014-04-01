using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public class TypeMapper : ITypeMapper
    {
        public MappingStrategy MappingStrategy { get; private set; }

        public TypeMapper()
            : this(MappingStrategy.Default)
        {}

        public TypeMapper(MappingStrategy mappingStrategy)
        {
            this.MappingStrategy = mappingStrategy;
        }

        public TTarget DeepClone<TTarget>(TTarget source)
            where TTarget : class, new()
        {
            if (source == null)
                return null;

            return (TTarget)MapInternal(source, typeof(TTarget), new MappingContext());
        }

        public object DeepClone(object source)
        {
            if (source == null)
                return null;

            var sourceType = source.GetType();

            return MapInternal(source, sourceType, new MappingContext());
        }

        public object DeepClone(object source, Type sourceType)
        {
            if (source == null)
                return null;

            return MapInternal(source, sourceType, new MappingContext());
        }

        object CreateClonePrototype(Type type)
        {
            var clone = Activator.CreateInstance(type);
            
            return clone;
        }

        bool IsBuiltInSimpleValueType(object obj)
        {
            return obj is sbyte
                || obj is byte
                || obj is short
                || obj is ushort
                || obj is int
                || obj is uint
                || obj is long
                || obj is ulong
                || obj is float
                || obj is double
                || obj is decimal
                || obj is string
                || obj is Enum;
        }

        public void Map<TTarget>(object source, TTarget target) where TTarget : class, new()
        {
            if (source == null)
                throw new ArgumentNullException("source");

            MapInternal(source, target, source.GetType(), typeof(TTarget), new MappingContext());
        }

        public void Map(object source, object target, Type targetType)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            MapInternal(source, target, source.GetType(), targetType, new MappingContext());
        }

        object MapInternal(object source, Type targetType, MappingContext cx)
        {
            if (IsBuiltInSimpleValueType(source))
                return source;

            bool isCloneNew = false;

            var clone =
                cx.Objects_MappedFromTo.GetOrAdd(
                source,
                (_) =>
                {
                    isCloneNew = true;
                    return CreateClonePrototype(targetType);
                });

            var sourceType = source.GetType();

            if (isCloneNew)
                MapInternal(source, clone, sourceType, targetType, cx);

            return clone;
        }

        void MapInternal(object source, object target, Type sourceType, Type targetType, MappingContext cx)
        {
            // todo: anything needed here for IReadOnlyList support in 4.5?
            if (MappingStrategy.CloneListElements && source is IList && target is IList)
            {
                DeepCloneListElements(source as IList, target as IList, cx);
            }

            var mappings = MappingStrategy.GetMemberMappings(sourceType, targetType);

            for(int i = 0; i < mappings.Count; i++)
            {
                var m = mappings[i];

                var val = m.From.GetValue(source);

                if (val == null)
                {
                    m.To.SetValue(target, null);
                }
                else
                {
                    if (IsBuiltInSimpleValueType(val))
                    {
                        m.To.SetValue(target, val);
                    }
                    else
                    {
                        // value is not null and is reference type
                        // create a clone and assign to target
                        var memberType = Type.GetType(m.To.AssemblyQualifiedMemberTypeName);
                        var clone = MapInternal(val, memberType, cx);

                        m.To.SetValue(target, clone);
                    }
                }
            }
        }

        void DeepCloneListElements(IList source, IList target, MappingContext cx)
        {
            for (int i = 0; i < source.Count; i++)
            {
                var sourceItem = source[i];

                var targetItem = MapInternal(sourceItem, sourceItem.GetType(), cx);

                target.Add(targetItem);
            }
        }


        public TTarget Map<TTarget>(object source) where TTarget : class, new()
        {
            if (source == null)
                return null;

            return (TTarget)MapInternal(source, typeof(TTarget), new MappingContext());
        }

        public object Map(object source, Type targetType)
        {
            if (source == null)
                return null;

            return MapInternal(source, targetType, new MappingContext());
        }
    }
}
