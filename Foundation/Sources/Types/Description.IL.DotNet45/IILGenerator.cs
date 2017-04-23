using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Types.Description.IL
{
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
}
