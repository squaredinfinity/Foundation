using SquaredInfinity.Foundation.Types.Description;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using SquaredInfinity.Foundation.Types.Mapping.MemberMatching;
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

            return (TTarget)MapInternal(source, typeof(TTarget), MappingOptions.Default, new MappingContext());
        }

        public object DeepClone(object source)
        {
            if (source == null)
                return null;

            var sourceType = source.GetType();

            return MapInternal(source, sourceType, MappingOptions.Default, new MappingContext());
        }

        public object DeepClone(object source, Type sourceType)
        {
            if (source == null)
                return null;

            return MapInternal(source, sourceType, MappingOptions.Default, new MappingContext());
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
            Map<TTarget>(source, target, MappingOptions.Default);
        }

        public void Map<TTarget>(object source, TTarget target, MappingOptions options) where TTarget : class, new()
        {
            if (source == null)
                throw new ArgumentNullException("source");

            MapInternal(source, target, source.GetType(), typeof(TTarget), options, new MappingContext());
        }

        public void Map(object source, object target, Type targetType)
        {
            Map(source, target, targetType, MappingOptions.Default);
        }

        public void Map(object source, object target, Type targetType, MappingOptions options)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            MapInternal(source, target, source.GetType(), targetType, options, new MappingContext());
        }

        object MapInternal(object source, Type targetType, MappingOptions options, MappingContext cx)
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
                MapInternal(source, clone, sourceType, targetType, options, cx);

            return clone;
        }

        void MapInternal(object source, object target, Type sourceType, Type targetType, MappingOptions options, MappingContext cx)
        {
            var key = new TypeMappingStrategiesKey(sourceType, targetType);

            var ms = TypeMappingStrategies.GetOrAdd(key, _key => CreateDefaultTypeMappingStrategy(sourceType, targetType));

            // todo: anything needed here for IReadOnlyList support in 4.5?
            if (ms.CloneListElements && source is IList && target is IList)
            {
                DeepCloneListElements(source as IList, target as IList, options, cx);
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

                    if (val == null && options.IgnoreNulls)
                        continue;

                    if (val == null || IsBuiltInSimpleValueType(val))
                    {
                        member.SetValue(target, val);
                    }
                    else
                    {
                        var memberType = Type.GetType(member.AssemblyQualifiedMemberTypeName);
                        
                        val = MapInternal(val, memberType, options, cx);

                        member.SetValue(target, val);
                    }
                }
                catch (Exception ex)
                {
                    // todo: internally log mapping error
                }
            }
        }

        protected virtual ITypeMappingStrategy CreateDefaultTypeMappingStrategy(Type sourceType, Type targetType)
        {
            var result =
                new TypeMappingStrategy(
                    sourceType,
                    targetType,
                    new ReflectionBasedTypeDescriptor(),
                    new MemberMatchingRuleCollection() { new ExactNameMatchMemberMatchingRule() },
                    valueResolvers: null);

            result.CloneListElements = true;

            return result;
        }

        void DeepCloneListElements(IList source, IList target, MappingOptions options, MappingContext cx)
        {
            for (int i = 0; i < source.Count; i++)
            {
                var sourceItem = source[i];

                var targetItem = MapInternal(sourceItem, sourceItem.GetType(), options, cx);

                target.Add(targetItem);
            }
        }


        public TTarget Map<TTarget>(object source) where TTarget : class, new()
        {
            return Map<TTarget>(source, MappingOptions.Default);
        }

        public TTarget Map<TTarget>(object source, MappingOptions options) where TTarget : class, new()
        {
            if (source == null)
                return null;

            return (TTarget)MapInternal(source, typeof(TTarget), options, new MappingContext());
        }

        public object Map(object source, Type targetType)
        {
            return Map(source, targetType, MappingOptions.Default);
        }

        public object Map(object source, Type targetType, MappingOptions options)
        {
            if (source == null)
                return null;

            return MapInternal(source, targetType, options, new MappingContext());
        }
    }
}
