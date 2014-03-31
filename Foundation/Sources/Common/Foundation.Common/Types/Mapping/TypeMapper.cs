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
            : this(DefaultMappingStrategy)
        {}

        public TypeMapper(MappingStrategy mappingStrategy)
        {
            this.MappingStrategy = mappingStrategy;
        }

        static MappingStrategy DefaultMappingStrategy { get; set; }
        
        static TypeMapper()
        {
            DefaultMappingStrategy = new MappingStrategy
            {
                CopyListElements = true
            };
        }

        public TTarget DeepClone<TTarget>(TTarget source)
            where TTarget : class, new()
        {
            if (source == null)
                return null;

            return (TTarget)DeepCloneInternal(source, typeof(TTarget), new MappingContext());
        }

        public object DeepClone(object source)
        {
            if (source == null)
                return null;

            var sourceType = source.GetType();

            return DeepCloneInternal(source, sourceType, new MappingContext());
        }

        public object DeepClone(object source, Type sourceType)
        {
            if (source == null)
                return null;

            return DeepCloneInternal(source, sourceType, new MappingContext());
        }

        object DeepCloneInternal(object source, Type targetType, MappingContext cx)
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

            if(isCloneNew)
                DeepCloneInternal(source, clone, targetType, targetType, cx);

            return clone;
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

            DeepCloneInternal(source, target, source.GetType(), typeof(TTarget), new MappingContext());
        }

        public void Map(object source, object target, Type targetType)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            DeepCloneInternal(source, target, source.GetType(), targetType, new MappingContext());
        }

        void DeepCloneInternal(object source, object target, Type sourceType, Type targetType, MappingContext cx)
        {
            // todo: anything needed here for IReadOnlyList support in 4.5?
            if (MappingStrategy.CopyListElements && source is IList && target is IList)
            {
                DeepCloneListElements(source as IList, target as IList, cx);
            }

            var mappings = MappingStrategy.GetMemberMappings(sourceType, targetType);

            for(int i = 0; i < mappings.Count; i++)
            {
                var m = mappings[i];
            }

            //var publicMembers =
            //    (from m in sourceDescription.Members
            //     where m.Visibility == MemberVisibility.Public
            //     select m).ToArray();

            //for (var i = 0; i < publicMembers.Length; i++)
            //{
            //    var member = publicMembers[i];

            //    if (!member.CanGetValue || !member.CanSetValue)
            //        continue;

            //    // get value etc
            //}

            var properties =
                (from p in sourceType.GetProperties()
                 where p.GetSetMethod() != null
                 && p.GetIndexParameters().Length == 0
                 select p).ToArray();

            foreach (var p in properties)
            {
                var val = p.GetValue(source, null);

                if (val == null)
                {
                    p.SetValue(target, null, null);
                }
                else
                {
                    if (IsBuiltInSimpleValueType(val))
                    {
                        p.SetValue(target, val, null);
                    }
                    else
                    {
                        // value is not null and is reference type
                        // create a clone and assign to target
                        var clone = DeepCloneInternal(val, p.PropertyType, cx);

                        p.SetValue(target, clone, null);
                    }
                }
            }
        }

        void DeepCloneListElements(IList source, IList target, MappingContext cx)
        {
            for (int i = 0; i < source.Count; i++)
            {
                var sourceItem = source[i];

                var targetItem = DeepCloneInternal(sourceItem, sourceItem.GetType(), cx);

                target.Add(targetItem);
            }
        }


        public TTarget Map<TTarget>(object source) where TTarget : class, new()
        {
            var target = (TTarget) CreateClonePrototype(typeof(TTarget));

            Map(source, target);

            return target;
        }

        public object Map(object source, Type targetType)
        {
            var target = CreateClonePrototype(targetType);

            Map(source, target, targetType);

            return target;
        }
    }
}
