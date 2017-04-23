using SquaredInfinity.Types.Description;
using SquaredInfinity.Types.Description.Reflection;
using SquaredInfinity.Types.Mapping.MemberMatching;
using SquaredInfinity.Types.Mapping.ValueResolving;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquaredInfinity.Extensions;
using System.Reflection;
using SquaredInfinity.Types.Description.IL;
using System.Threading;
using System.Data;

namespace SquaredInfinity.Types.Mapping
{
    public partial class TypeMapper : ITypeMapper
    {
        readonly TypeMappingStrategiesConcurrentDictionary TypeMappingStrategies = new TypeMappingStrategiesConcurrentDictionary();


        TypeResolver _typeResolver;
        TypeResolver TypeResolver
        {
            get 
            {
                if (_typeResolver == null)
                    _typeResolver = new TypeResolver();

                return _typeResolver; 
            }

            set
            {
                _typeResolver = value;
            }
        }

        ITypeDescriptor _typeDescriptor;
        ITypeDescriptor TypeDescriptor
        {
            get
            {
                if (_typeDescriptor == null)
                    _typeDescriptor = new ILBasedTypeDescriptor();

                return _typeDescriptor;
            }

            set
            {
                _typeDescriptor = value;
            }
        }
   
        public TypeMapper()
        {
            OnInitialize();
        }

        public TypeMapper(ITypeDescriptor typeDescriptor)
        {
            TypeDescriptor = typeDescriptor;

            OnInitialize();
        }

        public virtual void OnInitialize()
        {
            GetOrCreateTypeMappingStrategy<DataTable, DataTable>()
            .CreateTargetInstance((_source, _cx) =>
            {
                // clone
                var _clone = _source.Clone();

                // import rows
                foreach(var row in _source.Rows.OfType<DataRow>())
                {
                    _clone.ImportRow(row);
                }

                // mark as fully constructed so no further mapping is done
                _cx.MarkAsFullyConstructed();

                return _clone;
            });
        }

        #region Deep Clone

        public TTarget DeepClone<TTarget>(TTarget source)
        {
            if (source == null)
                return default(TTarget);

            return (TTarget)MapInternal(source, typeof(TTarget), MappingOptions.DefaultClone, new MappingContext(new CancellationToken()));
        }

        public TTarget DeepClone<TTarget>(TTarget source, MappingOptions mappingOptions)
        {
            if (source == null)
                return default(TTarget);

            return (TTarget)MapInternal(source, typeof(TTarget), mappingOptions, new MappingContext(new CancellationToken()));
        }

        public object DeepClone(object source)
        {
            if (source == null)
                return null;

            var sourceType = source.GetType();

            return MapInternal(source, sourceType, MappingOptions.DefaultClone, new MappingContext(new CancellationToken()));
        }

        public object DeepClone(object source, Type sourceType)
        {
            if (source == null)
                return null;

            return MapInternal(source, sourceType, MappingOptions.DefaultClone, new MappingContext(new CancellationToken()));
        }

        #endregion

        #region Map

        public object Map(object source, Type targetType)
        {
            return Map(source, targetType, MappingOptions.DefaultClone);
        }

        public object Map(object source, Type targetType, MappingOptions options)
        {
            if (source == null)
                return null;

            return MapInternal(source, targetType, options, new MappingContext(new CancellationToken()));
        }

        public object Map(object source, Type targetType, MappingOptions options, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Map(object source, object target, Type targetType)
        {
            Map(source, target, targetType, MappingOptions.DefaultClone, new CancellationToken());
        }

        public void Map(object source, object target, Type targetType, MappingOptions options)
        {
            Map(source, target, targetType, options, new CancellationToken());
        }

        public void Map(object source, object target, Type targetType, MappingOptions options, CancellationToken cancellationToken)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var sourceType = source.GetType();

            var key = new TypeMappingStrategyKey(sourceType, targetType);

            var ms = TypeMappingStrategies.GetOrAdd(key, (_) => CreateDefaultTypeMappingStrategy(sourceType, targetType));

            MapInternal(source, target, sourceType, targetType, ms, options, new MappingContext(cancellationToken));
        }

        #endregion

        #region Map<TTarget>

        public void Map<TTarget>(object source, TTarget target)
        {
            Map<TTarget>(source, target, MappingOptions.DefaultClone, new CancellationToken());
        }

        public void Map<TTarget>(object source, TTarget target, MappingOptions options)
        {
            Map<TTarget>(source, target, options, new CancellationToken());
        }

        public void Map<TTarget>(object source, TTarget target, MappingOptions options, CancellationToken cancellationToken)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var sourceType = source.GetType();
            var targetType = typeof(TTarget);

            var key = new TypeMappingStrategyKey(sourceType, targetType);

            var ms = TypeMappingStrategies.GetOrAdd(key, (_) => CreateDefaultTypeMappingStrategy(sourceType, targetType));

            MapInternal(source, target, sourceType, targetType, ms, options, new MappingContext(cancellationToken));
        }

        public TTarget Map<TTarget>(object source)
        {
            return Map<TTarget>(source, MappingOptions.DefaultClone);
        }

        public TTarget Map<TTarget>(object source, MappingOptions options)
        {
            if (source == null)
                return default(TTarget);

            return (TTarget)MapInternal(source, typeof(TTarget), options, new MappingContext(new CancellationToken()));
        }

        public TTarget Map<TTarget>(object source, MappingOptions options, CancellationToken cancellationToken)
        {
            if (source == null)
                return default(TTarget);

            return (TTarget)MapInternal(source, typeof(TTarget), options, new MappingContext(cancellationToken));
        }

        #endregion

        object MapInternal(object source, Type targetType, MappingOptions options, MappingContext cx, ITypeMappingStrategy ms = null)
        {
            var sourceType = source.GetType();

            if (ms == null)
            {
                ms = GetOrCreateTypeMappingStrategy(sourceType, targetType);
            }

            if(ms.CanCopyValueWithoutMapping)
                return source;

            var clone = (object)null;

            var create_cx = new CreateInstanceContext();

            if (options.TrackReferences && !targetType.IsValueType)
            {
                bool isCloneNew = false;

                clone =
                    cx.Objects_MappedFromTo.GetOrAdd(
                    source,
                    (_) =>
                    {
                        var _clone = (object)null;

                        if (ms.TryCreateInstace(source, ms.TargetTypeDescription, create_cx, out _clone))
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
                    MapInternal(source, clone, sourceType, targetType, ms, options, cx);
            }
            else
            {
                if (ms.TryCreateInstace(source, ms.TargetTypeDescription, create_cx, out clone))
                {
                    MapInternal(source, clone, sourceType, targetType, ms, options, cx);
                }
                else
                {
                    // todo: log error   
                }
            }

            return clone;
        }

        protected virtual void MapInternal(
            object source, 
            object target, 
            Type sourceType, 
            Type targetType, 
            ITypeMappingStrategy ms,
            MappingOptions options, 
            MappingContext cx)
        {
            if (cx.CancellationToken.IsCancellationRequested)
                return;

            var sourceList = source as IList;
            var targetList = target as IList;

            if (sourceList != null && targetList != null)
            {
                if(targetList.IsReadOnly)
                { 
                    // target list is read-only
                }

                var bulkUpdatesCollection = targetList as Collections.IBulkUpdatesCollection;
                
                IDisposable bulkUpdateOperation = (IDisposable)null;

                if (bulkUpdatesCollection != null)
                {
                    bulkUpdateOperation = bulkUpdatesCollection.BeginBulkUpdate();
                }

                if (options.ReuseTargetCollectionItemsWhenPossible && targetList.Count != 0 && sourceList.Count != 0)
                {
                    DeepCloneListElements(
                        sourceList,
                        targetList,
                        ms.SourceTypeDescription as IEnumerableTypeDescription,
                        ms.TargetTypeDescription as IEnumerableTypeDescription,
                        options,
                        cx);
                }
                else
                {
                    targetList.Clear();

                    DeepCloneListElements_DoNotReuseItems(
                        sourceList,
                        targetList,
                        ms.SourceTypeDescription as IEnumerableTypeDescription,
                        ms.TargetTypeDescription as IEnumerableTypeDescription,
                        options,
                        cx);
                }

                if (bulkUpdateOperation != null)
                    bulkUpdateOperation.Dispose();
            }

            foreach(var kvp in ms.TargetMembersMappings)
            {
                try
                {
                    if (cx.CancellationToken.IsCancellationRequested)
                        return;

                    var targetMemberDescription = kvp.Key;
                    var valueResolver = kvp.Value;

                    var mappedValueCandidate = (object)null;

                    if(!valueResolver.TryResolveValue(source, out mappedValueCandidate))
                    {
                        // unable to resolve source, skip this member
                        continue;
                    }

                    //# Check if simple assignment of a value from source to target can be made
                    //  e.g. when value is a value type without reference type properties / fields

                    if (targetMemberDescription.CanSetValue && mappedValueCandidate != null && valueResolver.CanCopyValueWithoutMapping)
                    {
                        targetMemberDescription.SetValue(target, mappedValueCandidate);
                        continue;
                    }


                    //// check if there exists value converter for source / target types
                    //if (!valueResolver.AreFromAndToTypesSame)
                    //{
                    //    //var converter = ms.TryGetValueConverter(valueResolver.ToType, targetMemberType);

                    //    //val = converter.Convert(val);
                    //}


                    //# mapping is not possible (source is null) or mapping mode is COPY
                    if (targetMemberDescription.CanSetValue && (mappedValueCandidate == null || options.Mode == MappingMode.Copy))
                    {
                        // if value is null and options are set to igonre nulls, then just skip this member and continue
                        if (options.IgnoreNulls)
                            continue;

                        targetMemberDescription.SetValue(target, mappedValueCandidate);
                        continue;
                    }

                    var targetMemberValue = targetMemberDescription.GetValue(target);

                    //# if source is null and was not handled before, this property cannot be mapped
                    if(mappedValueCandidate == null)
                    {
                        if(targetMemberValue == null)
                        {
                            // target is already null, can just skip
                            continue;
                        }

                        if(!targetMemberDescription.CanSetValue)
                        {
                            InternalTrace.Information(
                                () =>
                                    $"Unable to map property {targetMemberDescription.Name} on type {targetMemberDescription.DeclaringType.Name}. target property is read-only");
                        }

                        InternalTrace.Information(
                                () =>
                                    $"Unable to map property {targetMemberDescription.Name} on type {targetMemberDescription.DeclaringType.Name}.");

                        // skip mapping of this property
                        continue;
                    }

                    var sourceValType = mappedValueCandidate.GetType();
                    var targetValType = (Type)null;

                    if(!targetMemberDescription.CanSetValue && targetMemberValue == null)
                    {
                        // cannot set the value and cannot map anything because it's null
                        // just ignore and move on
                    }
                    else if (targetMemberDescription.CanSetValue && targetMemberValue == null)
                    {
                        // target value is null
                        // if source type and target type are compatible, clone source value
                        if (ms.IsToTypeAssignableFromFromType)
                        {
                            mappedValueCandidate = MapInternal(mappedValueCandidate, sourceValType, options, cx);
                            targetMemberDescription.SetValue(target, mappedValueCandidate);
                        }
                        else
                        {
                            if (targetMemberDescription.MemberType.Type.IsInterface)
                            {
                                // todo: log warning, unable to map source interface to target interface (don't which concrete type to use)
                            }
                            else
                            {
                                targetType = targetMemberDescription.MemberType.Type;

                                mappedValueCandidate = MapInternal(mappedValueCandidate, targetType, options, cx);
                                targetMemberDescription.SetValue(target, mappedValueCandidate);
                            }
                        }
                    }
                    else if (targetMemberDescription.CanSetValue && targetMemberDescription.MemberType.IsValueType)
                    {
                        mappedValueCandidate = MapInternal(mappedValueCandidate, targetMemberDescription.MemberType.Type, options, cx);
                        targetMemberDescription.SetValue(target, mappedValueCandidate);
                    }
                    else
                    {
                        // map
                        targetValType = targetMemberValue.GetType();

                        var _key = new TypeMappingStrategyKey(sourceValType, targetValType);
                        var _ms = TypeMappingStrategies.GetOrAdd(_key, (_) => CreateDefaultTypeMappingStrategy(sourceValType, targetValType));

                        var targetMemberValueAsIList = targetMemberValue as IList;

                        if (targetMemberValueAsIList != null)
                        {
                            if (options.ReuseTargetCollectionsWhenPossible && !targetMemberValueAsIList.IsReadOnly)
                            {
                                MapInternal(mappedValueCandidate, targetMemberValue, sourceValType, targetValType, _ms, options, cx);
                            }
                            else
                            {
                                if (!targetMemberDescription.CanSetValue)
                                {
                                    // cannot set value and target list is readonly
                                    // nothing we can do here
                                    // todo: log information
                                }
                                else
                                {
                                    // can set value, do not reuse target collection and recreate it instead
                                    var new_collection = MapInternal(mappedValueCandidate, targetValType, options, cx, _ms);

                                    targetMemberDescription.SetValue(target, new_collection);
                                }
                            }
                        }
                        else
                        {
                            if (targetMemberDescription.MemberType.Type.ImplementsInterface<IList>()) // or Icollection ?
                            {
                                if (options.ReuseTargetCollectionsWhenPossible)
                                {
                                    MapInternal(mappedValueCandidate, targetMemberValue, sourceValType, targetValType, _ms, options, cx);
                                }
                                else
                                {
                                    if (!targetMemberDescription.CanSetValue)
                                    {
                                        // cannot set value, resue target collection then
                                        MapInternal(mappedValueCandidate, targetMemberValue, sourceValType, targetValType, _ms, options, cx);
                                    }
                                    else
                                    {
                                        // can set value, do not reuse target collection and recreate it instead
                                        MapInternal(mappedValueCandidate, targetValType, options, cx, _ms);
                                    }
                                }
                            }
                            else
                            {
                                MapInternal(mappedValueCandidate, targetMemberValue, sourceValType, targetValType, _ms, options, cx);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        
        protected virtual ITypeMappingStrategy CreateDefaultTypeMappingStrategy(Type sourceType, Type targetType)
        {
            var descriptor = TypeDescriptor;

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

        ITypeMappingStrategy<TFrom, TTo> CreateDefaultTypeMappingStrategy<TFrom, TTo>()
        {
            var descriptor = TypeDescriptor;

            return CreateDefaultTypeMappingStrategy<TFrom, TTo>(
                descriptor,
                new MemberMatchingRuleCollection() { new ExactNameMatchMemberMatchingRule() },
               valueResolvers: null);
        }
        ITypeMappingStrategy<TFrom, TTo> CreateDefaultTypeMappingStrategy<TFrom, TTo>(
            ITypeDescriptor typeDescriptor,
            MemberMatchingRuleCollection memberMatchingRules,
            IEnumerable<IValueResolver> valueResolvers)
        {
            var result =
                new TypeMappingStrategy<TFrom, TTo>(
                    typeDescriptor,
                    typeDescriptor,
                    memberMatchingRules,
                    valueResolvers: null);

            return result;
        }

        void DeepCloneListElements(
            IList source, 
            IList target, 
            IEnumerableTypeDescription sourceTypeDescription, 
            IEnumerableTypeDescription targetTypeDescription, 
            MappingOptions options, 
            MappingContext cx)
        {
            var defaultConcreteItemType = targetTypeDescription.DefaultConcreteItemType;

            bool sourceItemOrTargetItemTypesChanged = false;

            var sourceItemType = (Type) null;
            var targetItemType = (Type) null;

            var itemsMappingStrategyKey = default(TypeMappingStrategyKey);
            var itemsMappingStrategy = (ITypeMappingStrategy)null;

            // trim target by removing not needed items from the end
            if (options.ReuseTargetCollectionItemsWhenPossible && target.Count > source.Count)
            {
                for (int i = target.Count - 1; i >= source.Count; i--)
                {
                    if (cx.CancellationToken.IsCancellationRequested)
                        return;

                    target.RemoveAt(i);
                }
            }

            if (targetTypeDescription.CanSetCapacity)
            {
                targetTypeDescription.SetCapacity(target, source.Count);
            }

            for (int i = 0; i < source.Count; i++)
            {
                if (cx.CancellationToken.IsCancellationRequested)
                    return;

                var sourceItem = source[i];

                if (sourceItem == null)
                {
                    target.Add(sourceItem);
                    continue;
                }

                bool isReusingTargetItem = false;
                var targetItem = (object)null;

                if (options.ReuseTargetCollectionItemsWhenPossible)
                {
                    if (target.Count - 1 >= i)
                    {
                        targetItem = target[i];

                        if (targetItem != null)
                        {
                            isReusingTargetItem = true;

                            var newSourceItemType = sourceItem.GetType();
                            var newTargetItemType = targetItem.GetType();

                            if (newSourceItemType != sourceItemType || newTargetItemType != targetItemType)
                            {
                                sourceItemOrTargetItemTypesChanged = true;
                            }

                            if (sourceItemOrTargetItemTypesChanged)
                            {
                                sourceItemType = newSourceItemType;
                                targetItemType = newTargetItemType;

                                itemsMappingStrategyKey = new TypeMappingStrategyKey(sourceItemType, targetItemType);

                                itemsMappingStrategy =
                                    TypeMappingStrategies
                                    .GetOrAdd(
                                    itemsMappingStrategyKey,
                                    CreateDefaultTypeMappingStrategy(sourceItemType, targetItemType));
                            }

                            // reuse target
                            MapInternal(sourceItem, targetItem, sourceItemType, targetItemType, itemsMappingStrategy, options, cx);
                            continue;
                        }
                    }
                }

                if (cx.CancellationToken.IsCancellationRequested)
                    return;

                // did not reuse target, replace it with new instance
                if (targetItem == null)
                {
                    if (targetTypeDescription.CanAcceptItemType(sourceItemType))
                    {
                        // target can accept source item
                        targetItem = MapInternal(sourceItem, sourceItemType, options, cx);
                    }
                    else
                    {
                        targetItem = MapInternal(sourceItem, defaultConcreteItemType, options, cx);
                    }
                }

                // if item in *i* position exists, replace it (as we failed to reuse it before)
                if (target.Count > i)
                {
                    if(!isReusingTargetItem)
                        target[i] = targetItem;
                }
                else
                {
                    target.Insert(i, targetItem);
                }
            }
        }

        void DeepCloneListElements_DoNotReuseItems(
            IList source,
            IList target,
            IEnumerableTypeDescription sourceTypeDescription,
            IEnumerableTypeDescription targetTypeDescription,
            MappingOptions options,
            MappingContext cx)
        {
            var sourceItem = (object)null;
            var targetItem = (object)null;

            var defaultConcreteItemType = targetTypeDescription.DefaultConcreteItemType;

            if(targetTypeDescription.CanSetCapacity)
            {
                targetTypeDescription.SetCapacity(target, source.Count);
            }

            var ms = (ITypeMappingStrategy)null;
            var canAcceptItemType = false;
            var last_sourceItemType = (Type)null;
            var sourceItemType = (Type) null;

            for (int i = 0; i < source.Count; i++)
            {
                if (cx.CancellationToken.IsCancellationRequested)
                    return;

                sourceItem = source[i];

                if (sourceItem == null)
                {
                    target.Add(null);

                    continue;
                }

                sourceItemType = sourceItem.GetType();

                if(last_sourceItemType != sourceItemType)
                {
                    last_sourceItemType = sourceItemType;

                    ms = GetOrCreateTypeMappingStrategy(sourceItemType, sourceItemType);

                    canAcceptItemType = targetTypeDescription.CanAcceptItemType(sourceItemType);
                }

                if (canAcceptItemType)
                {
                    targetItem = MapInternal(sourceItem, sourceItemType, options, cx, ms);
                }
                else
                {
                    targetItem = MapInternal(sourceItem, defaultConcreteItemType, options, cx, ms);
                }
                
                target.Insert(i, targetItem);
            }
        }

        ITypeMappingStrategy GetOrCreateTypeMappingStrategy(Type from, Type to)
        {
            var key = new TypeMappingStrategyKey(from, to);

            var result = TypeMappingStrategies.GetOrAdd(key, CreateDefaultTypeMappingStrategy(from, to));

            return result;
        }

        public ITypeMappingStrategy<TFrom, TTo> GetOrCreateTypeMappingStrategy<TFrom, TTo>()
        {
            return GetOrCreateTypeMappingStrategy<TFrom, TTo>(() => CreateDefaultTypeMappingStrategy<TFrom, TTo>());
        }

        public ITypeMappingStrategy<TFrom, TTo> GetOrCreateTypeMappingStrategy<TFrom, TTo>(Func<ITypeMappingStrategy<TFrom, TTo>> create)
        {
            var key = new TypeMappingStrategyKey(typeof(TFrom), typeof(TTo));

            var result = TypeMappingStrategies.GetOrAdd(key, create());

            if(!(result is ITypeMappingStrategy<TFrom, TTo>))
            {
                // not a generic TypeMappingStrategy
                // this was created automatically (before this method was called) during previous calls to Map()
                // replace with generic version which can be setup by user

                result = create();
            }

            return (ITypeMappingStrategy<TFrom, TTo>) result;
        }
    }
}
