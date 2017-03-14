using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation
{
    class NotifyPropertyChangedObjectWrapper<TSource> : 
        DynamicObject, 
        INotifyPropertyChanged 
        where TSource : class
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TSource Source { get; private set; }

        public NotifyPropertyChangedObjectWrapper(TSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            this.Source = source;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var propertyName = binder.Name;

            result = 
                Source.GetType()
                .GetProperty(propertyName)
                .GetValue(Source, null);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var propertyName = binder.Name;

            Source.GetType()
                .GetProperty(propertyName)
                .SetValue(Source, value, null);

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var methodName = binder.Name;

            result = 
                Source.GetType()
                .GetMethod(methodName)
                .Invoke(Source, args);

            return true;
        }

        public override string ToString()
        {
            return 
                Source
                .ToString();
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            var otherAsWrapper = other as NotifyPropertyChangedObjectWrapper<TSource>;

            if (otherAsWrapper != null)
            {
                return object.Equals(Source, otherAsWrapper.Source);
            }

            return object.Equals(Source, other);
        }

        public override int GetHashCode()
        {
            return Source.GetHashCode();
        }

        public static implicit operator NotifyPropertyChangedObjectWrapper<TSource>(TSource source)
        {
            return new NotifyPropertyChangedObjectWrapper<TSource>(source);
        }

        public static implicit operator TSource(NotifyPropertyChangedObjectWrapper<TSource> wrapper)
        {
            if (wrapper == null)
            {
                return default(TSource);
            }

            return wrapper.Source;
        }
    }
}