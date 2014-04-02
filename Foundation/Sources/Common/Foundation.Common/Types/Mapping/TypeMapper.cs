using SquaredInfinity.Foundation.Types.Description;
using SquaredInfinity.Foundation.Types.Mapping.ValueResolving;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    class TypeMappingStrategiesConcurrentDictionary
        : ConcurrentDictionary<TypeMappingStrategiesKey, ITypeMappingStrategy> { }

    struct TypeMappingStrategiesKey : IEquatable<TypeMappingStrategiesKey>
    {
        public Type SourceType;
        public Type TargetType;

        public TypeMappingStrategiesKey(Type source, Type target)
        {
            this.SourceType = source;
            this.TargetType = target;
        }

        public bool Equals(TypeMappingStrategiesKey other)
        {
            return
                SourceType == other.SourceType
                && TargetType == other.TargetType;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TypeMappingStrategy);
        }
    }

    public class TypeMapper : ITypeMapper
    {   
        TypeMappingStrategiesConcurrentDictionary TypeMappingStrategies = new TypeMappingStrategiesConcurrentDictionary();
        
        public TypeMapper()
        {
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

        protected virtual object CreateClonePrototype(Type type)
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

            
           //var clone = CloneOrReuseExistingInstance(source, targetType, cx);

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
            var key = new TypeMappingStrategiesKey(sourceType, targetType);

            var ms = TypeMappingStrategies.GetOrAdd(key, _key => TypeMappingStrategy.CreateTypeMappingStrategy(sourceType, targetType));

            // todo: anything needed here for IReadOnlyList support in 4.5?
            if (ms.CloneListElements && source is IList && target is IList)
            {
                DeepCloneListElements(source as IList, target as IList, cx);
            }

            for (int i = 0; i < ms.TargetTypeDescription.Members.Count; i++)
            {
                try
                {
                    var member = ms.TargetTypeDescription.Members[i];

                    var valueResolver = (IValueResolver)null;

                    if (!ms.TryGetValueResolverForMember(member.Name, out valueResolver))
                        continue;

                    var val = valueResolver.ResolveValue(source);

                    if (val == null || IsBuiltInSimpleValueType(val))
                    {
                        member.SetValue(target, val);
                    }
                    else
                    {
                        var memberType = Type.GetType(member.AssemblyQualifiedMemberTypeName);
                        
                        val = MapInternal(val, memberType, cx);

                        member.SetValue(target, val);
                    }
                }
                catch (Exception ex)
                {
                    // todo: internally log mapping error
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
