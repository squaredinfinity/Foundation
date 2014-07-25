using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Types.Description.IL
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ILBasedTypeMemberDescription : TypeMemberDescription
    {
        MemberInfo MemberInfo { get; set; }
        PropertyInfo PropertyInfo { get; set; }
        FieldInfo FieldInfo { get; set; }

        Action<object, object> SetPropertyValueDelegate;
        Func<object, object> GetPropertyValueDelegate;

        public ILBasedTypeMemberDescription(MemberInfo memberInfo)
        {
            this.MemberInfo = memberInfo;

            if (memberInfo is PropertyInfo)
            {
                PropertyInfo = memberInfo as PropertyInfo;

                SetPropertyValueDelegate = CreateSetValueDelegate(PropertyInfo);
                GetPropertyValueDelegate = CreateGetValueDelegate(PropertyInfo);
            }
            else if (memberInfo is FieldInfo)
            {
                FieldInfo = memberInfo as FieldInfo;
            }
        }

        public override object GetValue(object obj)
        {
            if (PropertyInfo != null)
            {
                return GetPropertyValueDelegate(obj);
            }
            else if (FieldInfo != null)
            {
                return FieldInfo.GetValue(obj);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public override void SetValue(object obj, object value)
        {
            if (PropertyInfo != null)
            {
                SetPropertyValueDelegate(obj, value);
            }
            else if (FieldInfo != null)
            {
                FieldInfo.SetValue(obj, value);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        Func<object, object> CreateGetValueDelegate(PropertyInfo propertyInfo)
        {
            MethodInfo getMethod = propertyInfo.GetGetMethod();

            if (getMethod == null)
            {
                return null;
            }

            Type[] argTypes = new Type[] { typeof(object) };

            DynamicMethod method = new DynamicMethod("__dynamicMethod_Get_" + propertyInfo.Name, typeof(object), argTypes);

            ILGenerator IL = method.GetILGenerator();

            IL.DeclareLocal(typeof(object));

            IL.Emit(OpCodes.Ldarg_0);

            IL.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);

            if (getMethod.IsVirtual)
                IL.Emit(OpCodes.Callvirt, getMethod);
            else
                IL.Emit(OpCodes.Call, getMethod);

            if (!propertyInfo.PropertyType.IsClass)
                IL.Emit(OpCodes.Box, propertyInfo.PropertyType);

            IL.Emit(OpCodes.Ret);


            var del = method.CreateDelegate(typeof(Func<object, object>));

            return (Func<object, object>)del;
        }

        Action<object, object> CreateSetValueDelegate(PropertyInfo propertyInfo)
        {
            MethodInfo setMethod = propertyInfo.GetSetMethod();
            
            if (setMethod == null)
            {
                return null;
            }

            Type[] argTypes = new Type[] { typeof(object), typeof(object) };

            DynamicMethod method = new DynamicMethod("__dynamicMethod_Set_" + propertyInfo.Name, null, argTypes);

            ILGenerator IL = method.GetILGenerator();

            IL.Emit(OpCodes.Ldarg_0);

            IL.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);

            IL.Emit(OpCodes.Ldarg_1);

            if (propertyInfo.PropertyType.IsClass)
                IL.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
            else
                IL.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);


            if (setMethod.IsVirtual)
                IL.Emit(OpCodes.Callvirt, setMethod);
            else
                IL.Emit(OpCodes.Call, setMethod);

            IL.Emit(OpCodes.Ret);

            method.DefineParameter(1, ParameterAttributes.In, "property_owner_instance");
            method.DefineParameter(2, ParameterAttributes.In, "property_value");

            var del = method.CreateDelegate(typeof(Action<object, object>));

            return (Action<object, object>)del;
        }
    }
}
