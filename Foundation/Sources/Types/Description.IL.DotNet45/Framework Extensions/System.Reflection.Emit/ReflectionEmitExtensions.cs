using SquaredInfinity.Types.Description.IL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
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
}
