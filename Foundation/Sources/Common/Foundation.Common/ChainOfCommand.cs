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

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        var grand_parent = new HandlesReferencesOfClonedObjects();
    //        grand_parent.Id = 1;
    //        grand_parent.GrandParent = null;

    //        var child = new HandlesReferencesOfClonedObjects();
    //        child.Id = 2;
    //        child.GrandParent = null;
    //        grand_parent.Child = child;

    //        var grand_child = new HandlesReferencesOfClonedObjects();
    //        grand_child.Id = 3;
    //        grand_child.GrandParent = grand_parent;
    //        grand_child.Child = null;

    //        child.Child = grand_child;

    //        var tm = new TypeMapper();
            
    //        List<object> list = new List<object>();

    //        list.Add(7);
    //        list.Add(grand_parent);
    //        list.Add(child);
    //        list.Add(grand_child);
    //        list.Add(13);

    //        var clone = tm.DeepClone(list);

    //        Console.ReadLine();
    //    }
    //}

    


 

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
