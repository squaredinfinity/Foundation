using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Linq.Expressions;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SquaredInfinity.Foundation.Serialization
{
    public abstract partial class FlexiSerializer
        : IFlexiSerializer
    {
        readonly TypeSerializationStrategiesConcurrentDictionary TypeSerializationStrategies =
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

        public FlexiSerializer()
        {
            ConfigureDefaultStrategies();
        }

        protected virtual void ConfigureDefaultStrategies()
        {
            
            
        }

        ITypeSerializationStrategy CreateDefaultTypeSerializationStrategy(Type type)
        {
            //if(type.IsGenericType && !type.IsGenericTypeDefinition)
            //{
            //    var generic_type_definition = type.GetGenericTypeDefinition();

            //    return CreateDefaultTypeSerializationStrategy(generic_type_definition, TypeDescriptor);
            //}

            return CreateDefaultTypeSerializationStrategy(type, TypeDescriptor);
        }

        protected abstract ITypeSerializationStrategy CreateDefaultTypeSerializationStrategy(
            Type type,
            ITypeDescriptor typeDescriptor);

        public ITypeSerializationStrategy GetOrCreateTypeSerializationStrategy(Type type)
        {
            var result = (ITypeSerializationStrategy)null;

            if (!type.IsGenericType)
            {
                return TypeSerializationStrategies
                    .GetOrAdd(type, _ => CreateDefaultTypeSerializationStrategy(type));
            }
            else if(type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                
                if(TypeSerializationStrategies.TryGetValue(type, out result))
                {
                    // strategy for requested type exists already, just return
                    return result;
                }
                else
                {
                    // strategy for requested type does not exist, create a new strategy and copy settings from generic type definition strategy (it if exists)

                    result = CreateDefaultTypeSerializationStrategy(type);

                    if(!TypeSerializationStrategies.TryAdd(type, result))
                    {
                        throw new InvalidOperationException();
                    }

                    return result;

                    // intially generic type definition support seemed like a good idea
                    // may need to go back to that at some point

                    //var generic_type_definition = type.GetGenericTypeDefinition();

                    //var generic_type_definition_strategy = (ITypeSerializationStrategy)null;

                    //if (TypeSerializationStrategies.TryGetValue(generic_type_definition, out generic_type_definition_strategy))
                    //{
                    //    result.CopySerializationSetupFrom(generic_type_definition_strategy);

                    //    return result;
                    //}

                    //return result;
                }
            }
            else // generic type definition
            {
                throw new NotImplementedException();

                if(TypeSerializationStrategies.TryGetValue(type, out result))
                {
                    return result;
                }
                else
                {
                    var generic_type_definition = type.GetGenericTypeDefinition();

                    return TypeSerializationStrategies
                        .GetOrAdd(generic_type_definition, _ => CreateDefaultTypeSerializationStrategy(generic_type_definition));
                }
            }
        }

        protected ITypeSerializationStrategy GetOrCreateTypeSerializationStrategy(
            Type type,
            Func<ITypeSerializationStrategy> create)
        {
            return TypeSerializationStrategies
                .GetOrAdd(type, _ => create());
        }

        public ITypeSerializationStrategy GetTypeSerializationStrategy(Type type)
        {
            return GetOrCreateTypeSerializationStrategy(type);
        }
    }
}
