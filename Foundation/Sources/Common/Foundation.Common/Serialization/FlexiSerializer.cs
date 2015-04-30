using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Linq.Expressions;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SquaredInfinity.Foundation.Serialization
{
    public abstract partial class FlexiSerializer : IFlexiSerializer
    {
        readonly protected TypeSerializationStrategiesConcurrentDictionary TypeSerializationStrategies =
            new TypeSerializationStrategiesConcurrentDictionary();

        TypeResolver _typeResolver;
        protected TypeResolver TypeResolver
        {
            get
            {
                if (_typeResolver == null)
                    _typeResolver = SquaredInfinity.Foundation.TypeResolver.Default;

                return _typeResolver;
            }
        }

        ITypeDescriptor _typeDescriptor;
        protected ITypeDescriptor TypeDescriptor
        {
            get
            {
                if (_typeDescriptor == null)
                    _typeDescriptor = Types.Description.TypeDescriptor.Default;

                return _typeDescriptor;
            }

            set
            {
                _typeDescriptor = value;
            }
        }

        public Func<Type, CreateInstanceContext, object> CustomCreateInstanceWith { get; set; }

        // todo context should be local to serialzation (type)
        // todo: this may also try to reuse type mapper code rather than copy it
        protected bool TryCreateInstace(Type targetType, CreateInstanceContext create_cx, out object newInstance)
        {
            newInstance = null;

            try
            {
                if (CustomCreateInstanceWith != null)
                {
                    try
                    {
                        newInstance = CustomCreateInstanceWith(targetType, create_cx);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }

                var constructor = targetType
                    .GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    types: Type.EmptyTypes,
                    modifiers: null);

                if (constructor != null)
                {
                    newInstance = constructor.Invoke(null);

                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.TryAddContextData("target type", () => targetType.FullName);
                InternalTrace.Information(ex, "Failed to create instance of type.");
            }

            return false;
        }

        public ITypeSerializationStrategy GetTypeSerializationStrategy(Type type)
        {
            return GetOrCreateTypeSerializationStrategy(type, () => CreateDefaultTypeSerializationStrategy(type));
        }

        protected ITypeSerializationStrategy CreateDefaultTypeSerializationStrategy(Type type)
        {
            return CreateDefaultTypeSerializationStrategy(type, TypeDescriptor);
        }

        protected abstract ITypeSerializationStrategy CreateDefaultTypeSerializationStrategy(
            Type type,
            ITypeDescriptor typeDescriptor);

        public ITypeSerializationStrategy<T> GetOrCreateTypeSerializationStrategy<T>()
        {
            return GetOrCreateTypeSerializationStrategy<T>(() => CreateDefaultTypeSerializationStrategy<T>());
        }

        public ITypeSerializationStrategy<T> GetOrCreateTypeSerializationStrategy<T>(
            Func<ITypeSerializationStrategy<T>> create)
        {
            var key = typeof(T);

            return (ITypeSerializationStrategy<T>)TypeSerializationStrategies
                .GetOrAdd(key, (ITypeSerializationStrategy)create());
        }

        public IEnumerableTypeSerializationStrategy<T, I> GetOrCreateEnumerableTypeSerializationStrategy<T, I>()
    where T : IEnumerable<I>
        {
            return (IEnumerableTypeSerializationStrategy<T, I>)GetOrCreateTypeSerializationStrategy<T>(() => CreateDefaultTypeSerializationStrategy<T, I>());
        }

        public ITypeSerializationStrategy GetOrCreateTypeSerializationStrategy(
            Type type,
            Func<ITypeSerializationStrategy> createDefault)
        {
            return (ITypeSerializationStrategy)TypeSerializationStrategies
                .GetOrAdd(type, createDefault());
        }

        public ITypeSerializationStrategy GetOrCreateTypeSerializationStrategy(
            Type type)
        {
            return (ITypeSerializationStrategy)TypeSerializationStrategies
                .GetOrAdd(type, CreateDefaultTypeSerializationStrategy(type));
        }

        ITypeSerializationStrategy<T> CreateDefaultTypeSerializationStrategy<T>()
        {
            return CreateDefaultTypeSerializationStrategy<T>(
                TypeDescriptor);
        }

        IEnumerableTypeSerializationStrategy<T, I> CreateDefaultTypeSerializationStrategy<T, I>()
            where T : IEnumerable<I>
        {
            return CreateDefaultTypeSerializationStrategy<T, I>(
                TypeDescriptor);
        }


        protected abstract IEnumerableTypeSerializationStrategy<T, I> CreateDefaultTypeSerializationStrategy<T, I>(
            ITypeDescriptor typeDescriptor) where T : IEnumerable<I>;

        protected abstract ITypeSerializationStrategy<T> CreateDefaultTypeSerializationStrategy<T>(
            ITypeDescriptor typeDescriptor);
    }
}
