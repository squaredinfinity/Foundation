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
        public T DeepClone<T>(T source)
            where T : class, new()
        {
            if (source == null)
                return null;

            return (T)DeepCloneInternal(source, typeof(T), new MappingContext());
        }

        public object DeepClone(object source)
        {
            if (source == null)
                return null;

            var sourceType = source.GetType();

            return DeepCloneInternal(source, sourceType, new MappingContext());
        }

        object DeepCloneInternal(object source, Type sourceType, MappingContext cx)
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
                        return CreateClonePrototype(source, sourceType, cx);
                    });

            if(isCloneNew)
                Map(source, clone, sourceType, sourceType, cx);

            return clone;
        }

        object CreateClonePrototype(object source, Type sourceType, MappingContext cx)
        {
            var clone = Activator.CreateInstance(sourceType);
            
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

        //public void Map<TSource, TTarget>(TSource source, TTarget target)// + strategy (e.g. map same properties only)
        //{
        //    Map(source, target, typeof(TSource), typeof(TTarget));
        //}

        //void Map(object source, object target, Type sourceType, Type targetType)
        //{
        //    var cx = new MappingContext();

        //    cx.Objects_MappedFromTo.Add(source, target);

        //    Map(source, target, sourceType, targetType, cx);
        //}

        void Map(object source, object target, Type sourceType, Type targetType, MappingContext cx)
        {
            if (source is IList && target is IList)
            {
                Map(source as IList, target as IList, cx);
            }

            //ITypeDescriptor td = new SquaredInfinity.Foundation.Types.Description.Reflection.ReflectionBasedTypeDescriptor();

            //var sourceDescription = td.DescribeType(sourceType);

            //for (var i = 0; i < sourceDescription.Members.Count; i++)
            //{
            //    var member = sourceDescription.Members[i];

            //    //if (!member.Accessor.CanGetValue || !member.Accessor.CanSetValue)
            //    //    continue;

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

        void Map(IList source, IList target, MappingContext cx)
        {
            for (int i = 0; i < source.Count; i++)
            {
                var sourceItem = source[i];

                var targetItem = DeepCloneInternal(sourceItem, sourceItem.GetType(), cx);

                target.Add(targetItem);
            }
        }

        //ITypeDescription DescribeType(string assemblyQualifiedTypeName)
        //{
        //    // todo
        //    ITypeDescriptor descriptor = new ReflectionBasedTypeDescriptor();


        //    return null;
        //}
    }
}
