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
using SquaredInfinity.Foundation.Extensions;
using System.Reflection;

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

        public override int GetHashCode()
        {
            var hash = 21;

            hash *= 31 ^ SourceType.GetHashCode();
            hash *= 31 ^ TargetType.GetHashCode();

            return hash;
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

            var sourceType = source.GetType();

            var key = new TypeMappingStrategiesKey(sourceType, targetType);

            var ms = TypeMappingStrategies.GetOrAdd(key, _key => CreateDefaultTypeMappingStrategy(sourceType, targetType));

            bool isCloneNew = false;

            var create_cx = new CreateInstanceContext();

            var clone =
                cx.Objects_MappedFromTo.GetOrAdd(
                source,
                (_) =>
                {
                    var _clone = (object) null;

                    if(ms.TryCreateInstace(source, targetType, create_cx, out _clone))
                    {
                        isCloneNew = true;
                        return _clone;
                    }
                    else
                    {
                        // todo: log error
                        return source;
                    }
                });

            if (isCloneNew && !create_cx.IsFullyConstructed)
                MapInternal(source, clone, sourceType, targetType, options, cx);

            return clone;
        }

        protected virtual void MapInternal(object source, object target, Type sourceType, Type targetType, MappingOptions options, MappingContext cx)
        {
            var key = new TypeMappingStrategiesKey(sourceType, targetType);

            var ms = TypeMappingStrategies.GetOrAdd(key, _key => CreateDefaultTypeMappingStrategy(sourceType, targetType));

            // todo: anything needed here for IReadOnlyList support in 4.5?
            if (source is IList && target is IList)
            {
                if (!options.ReuseTargetCollectionItemsWhenPossible)
                    (target as IList).Clear();

                DeepCloneListElements(source as IList, target as IList, options, cx);
            }

            for (int i = 0; i < ms.TargetTypeDescription.Members.Count; i++)
            {
                try
                {
                    var targetMember = ms.TargetTypeDescription.Members[i];

                    var valueResolver = (IValueResolver)null;

                    if (!ms.TryGetValueResolverForMember(targetMember.Name, out valueResolver))
                        continue;

                    var val = valueResolver.ResolveValue(source);
                    
                    // check if there exists value converter for source / target types

                    var targetMemberType = Type.GetType(targetMember.AssemblyQualifiedMemberTypeName);

                    if (valueResolver.ToType != targetMemberType)
                    {
                        //var converter = ms.TryGetValueConverter(valueResolver.ToType, targetMemberType);

                        //val = converter.Convert(val);
                    }

                    if (val == null && options.IgnoreNulls)
                        continue;

                    if (val == null || IsBuiltInSimpleValueType(val))
                    {
                        targetMember.SetValue(target, val);
                    }
                    else
                    {
                        // todo: what if target is null ? should we create new instance ?
                        if (options.ReuseTargetCollectionsWhenPossible && targetMemberType.ImplementsInterface<IList>())
                        {
                            MapInternal(val, targetMember.GetValue(target), val.GetType(), targetMemberType, options, cx);
                        }
                        else
                        {
                            val = MapInternal(val, val.GetType(), options, cx);
                            targetMember.SetValue(target, val);
                        }
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
            var descriptor = new ReflectionBasedTypeDescriptor();

            var result =
                new TypeMappingStrategy(
                    sourceType,
                    targetType,
                    descriptor,
                    descriptor,
                    new MemberMatchingRuleCollection() { new ExactNameMatchMemberMatchingRule() },
                    valueResolvers: null);

            return result;
        }

        TypeMappingStrategy<TFrom, TTo> CreateDefaultTypeMappingStrategy<TFrom, TTo>(bool autoMatchMembers = true)
        {
            return CreateDefaultTypeMappingStrategy<TFrom, TTo>(
                new ReflectionBasedTypeDescriptor(),
               new MemberMatchingRuleCollection() { new ExactNameMatchMemberMatchingRule() },
               valueResolvers: null);
        }
        TypeMappingStrategy<TFrom, TTo> CreateDefaultTypeMappingStrategy<TFrom, TTo>(
            ITypeDescriptor typeDescriptor,
            MemberMatchingRuleCollection memberMatchingRules,
            IEnumerable<IValueResolver> valueResolvers)
        {
            var descriptor = new ReflectionBasedTypeDescriptor();

            var result =
                new TypeMappingStrategy<TFrom, TTo>(
                    typeDescriptor,
                    typeDescriptor,
                    memberMatchingRules,
                    valueResolvers: null);

            return result;
        }

        void DeepCloneListElements(IList source, IList target, MappingOptions options, MappingContext cx)
        {
            for (int i = 0; i < source.Count; i++)
            {
                var sourceItem = source[i];

                if (sourceItem == null)
                {
                    target.Add(sourceItem);
                    continue;
                }

                var targetItem = (object)null;

                if (options.ReuseTargetCollectionItemsWhenPossible)
                {
                    if (target.Count - 1 >= i)
                    {
                        targetItem = target[i];

                        if (targetItem != null)
                        {
                            var sourceItemType = sourceItem.GetType();
                            var targetItemType = targetItem.GetType();

                            if (sourceItemType == targetItemType)
                            {
                                // reuse target
                                MapInternal(sourceItem, targetItem, sourceItemType, targetItemType, options, cx);
                                continue;
                            }
                        }
                    }
                }

                // did not reuse target, replace it with new instance

                targetItem = MapInternal(sourceItem, sourceItem.GetType(), options, cx);

                // if item in *i* position exists, replace it (as we failed to reuse it above)
                if (target.Count > i)
                {
                    target.RemoveAt(i);
                }
                
                target.Insert(i, targetItem);
            }

            if(options.ReuseTargetCollectionItemsWhenPossible && target.Count > source.Count)
            {
                for(int i = source.Count; i < target.Count; i++)
                {
                    target.RemoveAt(i);
                }
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

        public object Map(object source, Type targetType, MappingOptions options, MappingContext cx)
        {
            if (source == null)
                return null;

            return MapInternal(source, targetType, options, cx);
        }

        public TypeMappingStrategy<TFrom, TTo> GetOrCreateTypeMappingStrategy<TFrom, TTo>(bool autoMatchMembers = true)
        {
            return GetOrCreateTypeMappingStrategy<TFrom, TTo>(() => CreateDefaultTypeMappingStrategy<TFrom, TTo>(autoMatchMembers));
        }

        public TypeMappingStrategy<TFrom, TTo> GetOrCreateTypeMappingStrategy<TFrom, TTo>(Func<TypeMappingStrategy<TFrom, TTo>> create)
        {
            var key = new TypeMappingStrategiesKey(typeof(TFrom), typeof(TTo));

            // todo: create T ITypeMappingStrategy instead
            return (TypeMappingStrategy<TFrom, TTo>)TypeMappingStrategies.GetOrAdd(key, (ITypeMappingStrategy) create());
        }
    }
}
