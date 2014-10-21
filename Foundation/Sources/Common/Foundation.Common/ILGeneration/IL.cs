using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.ILGeneration
{
    internal class IL : IILGenerator
    {
        readonly ILGenerator Generator;

        public IL(ILGenerator generator)
        {
            this.Generator = generator; 
        }

        public IILGenerator BeginCatchBlock(Type exceptionType)
        {
            Generator.BeginCatchBlock(exceptionType);
            return this;
        }

        public IILGenerator ldarg_0
        {
            get
            {
                Generator.Emit(OpCodes.Ldarg_0);
                return this;
            }
        }

        public IILGenerator ldarg_1
        {
            get
            {
                Generator.Emit(OpCodes.Ldarg_1);
                return this;
            }
        }

        public IILGenerator castclass(Type type)
        {
            Generator.Emit(OpCodes.Castclass, type);
            return this;
        }

        public IILGenerator call_or_callvirt(MethodInfo methodInfo)
        {
            if (methodInfo.IsVirtual)
                Generator.Emit(OpCodes.Callvirt, methodInfo);
            else
                Generator.Emit(OpCodes.Call, methodInfo);

            return this;
        }

        public IILGenerator box(Type type)
        {
            Generator.Emit(OpCodes.Box, type);
            return this;
        }

        public IILGenerator box_if_value_type(Type type)
        {
            if (type.IsValueType)
            {
                return box(type);
            }
            else
            {
                return this;
            }
        }

        public IILGenerator ret()
        {
            Generator.Emit(OpCodes.Ret);
            return this;
        }

        public LocalBuilder DeclareLocal(Type type, bool pinned)
		{
			return Generator.DeclareLocal(type, pinned);
		}

        public IILGenerator ldloca_s(byte index)
		{
			Generator.Emit(OpCodes.Ldloca_S, index);
            return this;
		}

        public IILGenerator initobj(Type type)
		{
			Generator.Emit(OpCodes.Initobj, type);
			return this;
		}

        public IILGenerator ldloc_0
		{
			get
			{
				Generator.Emit(OpCodes.Ldloc_0);
				return this;
			}
		}

        public IILGenerator end()
        {
            return this;
        }

        public IILGenerator newobj(ConstructorInfo ci)
        {
            Generator.Emit(OpCodes.Newobj, ci);
            return this;
        }


        public IILGenerator unbox_any(Type type)
        {
            Generator.Emit(OpCodes.Unbox_Any, type);
            return this;
        }

        public IILGenerator cast_object_to(Type targetType)
        {
            if (targetType == typeof(object))
                return this;

            if (targetType.IsValueType)
            {
                return unbox_any(targetType);
            }

            return castclass(targetType);
        }
    }

    public interface IILGenerator
    {


        IILGenerator BeginCatchBlock(Type exceptionType);

        IILGenerator ldarg_0 { get; }
        IILGenerator ldarg_1 { get; }
        IILGenerator castclass(Type type);
        IILGenerator call_or_callvirt(MethodInfo methodInfo);
        IILGenerator box(Type type);
        IILGenerator box_if_value_type(Type type);
        IILGenerator ret();
        LocalBuilder DeclareLocal(Type type, bool pinned);
        IILGenerator ldloca_s(byte index);
        IILGenerator initobj(Type type);
        IILGenerator ldloc_0 { get; }
        IILGenerator end();
        IILGenerator newobj(ConstructorInfo ci);
        IILGenerator unbox_any(Type type);
        IILGenerator cast_object_to(Type targetType);
    }

    public static class ReflectionEmitExtensions
    {
        public static PropertySetter EmitSetPropertyValueDelegate(this PropertyInfo pi)
        {
            var dynMeth = CreateDynamicMethod(
                "__SI_IL_set__" + pi.Name,
                pi.DeclaringType,
                null,
                new Type[] { typeof(object), typeof(object) });

            var IL = new IL(dynMeth.GetILGenerator());

            IL
                .ldarg_0
                .castclass(pi.DeclaringType)
                .ldarg_1
                .end();

            if (pi.PropertyType.IsClass)
            {
                IL
                    .castclass(pi.PropertyType);
            }
            else
                IL
                    .cast_object_to(pi.PropertyType);

            var setMethod = pi.GetSetMethod(nonPublic: true);

            IL
                    .call_or_callvirt(setMethod)
                    .ret();

            //dynMeth.DefineParameter(1, ParameterAttributes.In, "property_owner_instance");
            //dynMeth.DefineParameter(2, ParameterAttributes.In, "property_value");

            return (PropertySetter)dynMeth.CreateDelegate(typeof(PropertySetter));
        }

        public static PropertyGetter EmitGetPropertyValueDelegate(this PropertyInfo pi)
        {
            var dynMeth = CreateDynamicMethod(
                "__SI_IL_get__" + pi.Name,
                pi.DeclaringType,
                typeof(object),
                new Type[] { typeof(object) });

            var IL = new IL(dynMeth.GetILGenerator());


            bool isStatic = pi.IsStatic();

            if (!isStatic)
            {
                IL
                    .ldarg_0                          // load arg-0 (this)
                    .castclass(pi.DeclaringType);     // (PropertyOwnerType)this
            }

            var getMethod = pi.GetGetMethod(nonPublic: true);

            IL
                .call_or_callvirt(getMethod)
                .box_if_value_type(pi.PropertyType)
                .ret();

            return (PropertyGetter)dynMeth.CreateDelegate(typeof(PropertyGetter));
        }

        public static ConstructorInvocation EmitConstructorInvocationDelegate(this Type type)
        {
            var dynMeth = CreateDynamicMethod(
                "__SI_IL_ctor__" + type.Name,
                type,
                typeof(object),
                new Type[] { typeof(object) });

            var IL = new IL(dynMeth.GetILGenerator());

            if (type.IsValueType)
            {
                IL
                    .DeclareLocal(type, pinned: false);   // DeclaringType x

                IL
                    .ldloca_s(0)
                    .initobj(type)
                    .ldloc_0
                    .end();

                IL
                    .box_if_value_type(type)
                    .ret();
            }

            return (ConstructorInvocation)dynMeth.CreateDelegate(typeof(ConstructorInvocation));
        }

        public static ConstructorInvocation EmitConstructorInvocationDelegate(this ConstructorInfo ci)
        {
            var dynMeth = CreateDynamicMethod(
                "__SI_IL_ctor__" + ci.DeclaringType.Name,
                ci.DeclaringType,
                typeof(object),
                new Type[] { typeof(object) });

            var IL = new IL(dynMeth.GetILGenerator());

            var ciParameters = ci.GetParameters();

            IL
                .newobj(ci);

            // todo: parameters handling

            IL
                .box_if_value_type(ci.DeclaringType)
                .ret();

            return (ConstructorInvocation)dynMeth.CreateDelegate(typeof(ConstructorInvocation));
        }

        static DynamicMethod CreateDynamicMethod(string name, Type owner, Type returnType, Type[] parameterTypes)
        {
            if (owner.IsInterface)
            {
                owner = typeof(DynamicMethodOwnerSurrogate);
                name = owner.FullName.Replace('.', '_') + name;
            }

            var dynMeth =
                new DynamicMethod(
                    name,
                    MethodAttributes.Static | MethodAttributes.Public,
                    CallingConventions.Standard,
                    returnType,
                    parameterTypes,
                    owner,
                    skipVisibility: true);

            return dynMeth;
        }
    }

    class DynamicMethodOwnerSurrogate { }

    public delegate void PropertySetter(object obj, object value);

    public delegate object PropertyGetter(object obj);

    public delegate object ConstructorInvocation(params object[] parameters);
}

