using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    /// <summary>
    /// Provides an implementation of Chain Of Command pattern based on event handlers.
    /// Event handlers will be fired in a sequence.
    /// If event handler can handle given command it should set args.Handled = true when done.
    /// </summary>
    public static class ChainOfCommand
    {
        /// <summary>
        /// Provides an implementation of Chain Of Command pattern based on event handlers.
        /// Event handlers will be fired in a sequence.
        /// If event handler can handle given command it should set args.Handled = true when done.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="eventHandler"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool TryHandle<TEventArgs>(this EventHandler<TEventArgs> eventHandler, TEventArgs args)
            where TEventArgs : CommandHandlerEventArgs
        {
            if (eventHandler == null)
                return false;

            var invocationList =
                eventHandler
                .GetInvocationList();

            for (int i = 0; i < invocationList.Length; i++)
            
            {
                var del = invocationList[i] as EventHandler<TEventArgs>;

                del(null, args);

                if (args.IsHandled)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

    class Program
    {
        static void Main(string[] args)
        {
            var grand_parent = new HandlesReferencesOfClonedObjects();
            grand_parent.Id = 1;
            grand_parent.GrandParent = null;

            var child = new HandlesReferencesOfClonedObjects();
            child.Id = 2;
            child.GrandParent = null;
            grand_parent.Child = child;

            var grand_child = new HandlesReferencesOfClonedObjects();
            grand_child.Id = 3;
            grand_child.GrandParent = grand_parent;
            grand_child.Child = null;

            child.Child = grand_child;

            var tm = new TypeMapper();
            
            List<object> list = new List<object>();

            list.Add(7);
            list.Add(grand_parent);
            list.Add(child);
            list.Add(grand_child);
            list.Add(13);

            var clone = tm.DeepClone(list);

            Console.ReadLine();
        }
    }

    public class TypeMapper
    {

        public T DeepClone<T>(T source)
            where T : class, new()
        {
            if (source == null)
                return null;

            return (T) DeepClone(source, typeof(T), new MappingContext());
        }

        public object DeepClone(object source)
        {
            if (source == null)
                return null;

            var sourceType = source.GetType();

            return DeepClone(source, sourceType, new MappingContext());
        }

        object DeepClone(object source, Type sourceType, MappingContext cx)
        {            
            if (IsBuiltInSimpleValueType(source))
                return source;

            var clone = (object) null;

            if (cx.Objects_MappedFromTo.ContainsKey(source))
            {
                clone = cx.Objects_MappedFromTo[source];
            }
            else
            {
                clone = Activator.CreateInstance(sourceType);
                cx.Objects_MappedFromTo.Add(source, clone);
            }

            Map(source, clone, sourceType, sourceType, cx);

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
                || obj is decimal;
        }

        public void Map<TSource, TTarget>(TSource source, TTarget target)// + strategy (e.g. map same properties only)
        {
            Map(source, target, typeof(TSource), typeof(TTarget));
        }
        
        void Map(object source, object target, Type sourceType, Type targetType)
        {
            var cx = new MappingContext();

            cx.Objects_MappedFromTo.Add(source, target);

            Map(source, target, sourceType, targetType, cx);
        }

        void Map(object source, object target, Type sourceType, Type targetType, MappingContext cx)
        {
            if(source is IList && target is IList)
            {
                Map(source as IList, target as IList, cx);
            }

            ITypeDescriptor td = new SquaredInfinity.Foundation.Types.Description.Reflection.ReflectionBasedTypeDescriptor();

            var sourceDescription = td.DescribeType(sourceType);

            for(var i = 0; i < sourceDescription.Members.Count; i++)
            {
                var member = sourceDescription.Members[i];

                //if (!member.Accessor.CanGetValue || !member.Accessor.CanSetValue)
                //    continue;

                // get value etc
            }

            var properties =
                (from p in sourceType.GetProperties()
                 where p.GetSetMethod() != null
                 && p.GetIndexParameters().Length == 0
                 select p).ToArray();

            foreach(var p in properties)
            {
                var val = p.GetValue(source, null);

                if(val == null)
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
                        if (cx.Objects_MappedFromTo.ContainsKey(val))
                        {
                            p.SetValue(target, cx.Objects_MappedFromTo[val], null);
                        }
                        else
                        {
                            // map reference type
                            var newValue = Activator.CreateInstance(p.PropertyType);

                            cx.Objects_MappedFromTo.Add(val, newValue);

                            Map(val, newValue, p.PropertyType, p.PropertyType, cx);

                            p.SetValue(target, newValue, null);
                        }
                    }
                }
            }
        }

        void Map(IList source, IList target, MappingContext cx)
        {
            for(int i = 0; i < source.Count; i++)
            {
                var sourceItem = source[i];

                var targetItem = DeepClone(sourceItem, sourceItem.GetType(), cx);

                target.Add(targetItem);
            }
        }

        ITypeDescription DescribeType(string assemblyQualifiedTypeName)
        {
            // todo
            ITypeDescriptor descriptor = new ReflectionBasedTypeDescriptor();


            return null;
        }
    }


 

    // default conventions:
    //  flatten hierarchy
    //  match property names exactly
    //  match property names using ClassNamePropertyName (e.g. User.Id => UserId)
    //  methods starting with Getxxx() => xxx

    // other:
    //  null substitutions (e.g. null => "n\a")

    /// <summary>
    /// Declares rules used in mapping.
    /// For example, map only public properties with the same name.
    /// </summary>
    public interface IMappingConvention
    {
        IMemberAccessor MapMember(string sanitizedMemberName);
    }

    public class ExactPropertyNameMatchMappingConvention : IMappingConvention
    {
        //public IMemberAccessor MapMember(string sanitizedMemberName)
        //{
            
        //}
        public IMemberAccessor MapMember(string sanitizedMemberName)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// e.g. GetXXX(), SetXXX()
    /// </summary>
    public class MethodDrivenAccessor : IMemberAccessor
    {
        public string SanitizedName { get; set; }
        public bool CanGetValue { get; set; }
        public bool CanSetValue { get; set; }

        public void TrySetValue(object newValue)
        {
            throw new NotImplementedException();
        }

        public object TryGetValue()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// e.g. XXX { get; set; }
    /// </summary>
    public class PropertyAccessor : IMemberAccessor
    {
        public string SanitizedName { get; set; }
        public bool CanGetValue { get; set; }
        public bool CanSetValue { get; set; }

        public void TrySetValue(object newValue)
        {
            throw new NotImplementedException();
        }

        public object TryGetValue()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Defines details of how one type should be mapped to another type.
    /// </summary>
    public class TypeMappingStrategy
    {
        public Type Source { get; set; }
        public Type Target { get; set; }
    }

    public class PropertyMappingStrategy
    {
        public string SourcePropertyName { get; set; }
        public string TargetPropertyName { get; set; }
    }


    public interface IMemberValueConverter
    {
        object Convert(object memberValue);
    }



    //class MappingContext
    //{
    //    public readonly Dictionary<object, object> Objects_MappedFromTo = new Dictionary<object, object>();
    //}

    //[DebuggerDisplay("{DebuggerDisplay}")]
    //public class HandlesReferencesOfClonedObjects
    //{
    //    public string DebuggerDisplay
    //    {
    //        get
    //        {
    //            return string.Format("{0}, {1}", Id, GetHashCode());
    //        }
    //    }

    //    public int Id { get; set; }
    //    public HandlesReferencesOfClonedObjects Child { get; set; }
    //    public HandlesReferencesOfClonedObjects GrandParent { get; set; }
    //}
