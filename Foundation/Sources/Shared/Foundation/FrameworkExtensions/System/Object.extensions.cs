using SquaredInfinity.Types.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class ObjectExtensions
    {
        static readonly TypeMapper DefaultTypeMapper = new TypeMapper();

        public static void DeepCopyFrom<TTarget>(this TTarget target, object source)
        {
            target.DeepCopyFrom<TTarget>(source, MappingOptions.DefaultCopy);
        }

        public static void DeepCopyFrom<TTarget>(this TTarget target, object source, MappingOptions options)
        {
            DefaultTypeMapper.Map(source, target, options);
        }

        public static TTarget DeepClone<TTarget>(this TTarget source)
        {
            var targetType = typeof(TTarget);

            if (targetType.IsAbstract || targetType.IsInterface)
            {
                if (source != null && source is TTarget)
                {
                    return (TTarget)DefaultTypeMapper.DeepClone(source, source.GetType());
                }
                else
                {
                    throw new ArgumentException("Unable to clone to interface or abstract class.");
                }
            }
            else
            {
                return DefaultTypeMapper.DeepClone(source);
            }
        }

        public static object DeepClone(this object source, Type sourceType)
        {
            return DefaultTypeMapper.DeepClone(source, sourceType);
        }

        public static IEventSubscriptionPrototype<TEventSource> CreateWeakEventHandler<TEventSource>(this TEventSource eventSource)
        {
            return new EventSubscriptionPrototype<TEventSource>(eventSource);
        }

        public static bool TryConvert(this object obj, Type targetType, out object result)
        {
            result = null;

            if (obj != null)
            {
                var objType = obj.GetType();

                if (objType.IsTypeEquivalentTo(targetType, treatNullableAsEquivalent: true) || objType.ImplementsOrExtends(targetType))
                {
                    result = obj;
                    return true;
                }
            }

            if(targetType.IsEnum)
            {
                if (obj is string)
                {
                    result = Enum.Parse(targetType, obj as string);
                }
                else
                {
                    result = Enum.ToObject(targetType, obj);
                }

                return true;
            }

            // todo: test if conversion enum -> string, enum -> int etc are supported

            if (obj is IConvertible)
            {
                result = System.Convert.ChangeType(obj, targetType);
                return true;
            }

            var converter = TypeDescriptor.GetConverter(obj);

            var cx = new ConverterContext(obj);

            if(converter != null && converter.CanConvertTo(cx, targetType))
            {
                result = converter.ConvertTo(obj, targetType);
                return true;
            }

            return false;
        }

        class ConverterContext : ITypeDescriptorContext
        {
            public IContainer Container
            {
                get { return null; }
            }

            object _instance;
            public object Instance
            {
                get { return _instance; }
                private set { _instance = value; }
            }

            public ConverterContext(object instance)
            {
                this.Instance = instance;
            }

            public void OnComponentChanged()
            { }

            public bool OnComponentChanging()
            {
                return true;
            }

            public PropertyDescriptor PropertyDescriptor
            {
                get { return null; }
            }

            public object GetService(Type serviceType)
            {
                return null;
            }
        }

        public static object Convert(this object obj, Type targetType)
        {
            var result = (object)null;

            if (TryConvert(obj, targetType, out result))
                return result;

            throw new InvalidOperationException($"Unable to convert object to type {targetType.FullName}");
        }

        public static TTarget Convert<TTarget>(this object obj)
        {
            return (TTarget)Convert(obj, typeof(TTarget));
        }

        public static void SafeReleaseComObject(this object obj)
        {
            if (obj != null)
            {
                if (Marshal.IsComObject(obj))
                {
                    Marshal.ReleaseComObject(obj);
                }
            }
        }
    }
}
