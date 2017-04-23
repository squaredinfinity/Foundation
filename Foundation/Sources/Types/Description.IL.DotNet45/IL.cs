using System;
using System.Collections.Generic;
using SquaredInfinity.Extensions;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Types.Description.IL
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

    class DynamicMethodOwnerSurrogate { }

    public delegate void PropertySetter(object obj, object value);

    public delegate object PropertyGetter(object obj);

    public delegate object ConstructorInvocation(params object[] parameters);
}
