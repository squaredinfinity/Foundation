using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Types.Description.IL
{
    public class ILBasedTypeDescription : TypeDescription
    {
        // todo: use il

        ConstructorInfo ParameterLessConstructorInfo;

        public override object CreateInstance()
        {
            if (ParameterLessConstructorInfo == null)
            {
                ParameterLessConstructorInfo =
                    Type
                   .GetConstructor(
                   BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                   binder: null,
                   types: Type.EmptyTypes,
                   modifiers: null);
            }

            if (ParameterLessConstructorInfo == null)
            {
                throw new InvalidOperationException("Type {0} does not have parameterless contructor".FormatWith(Type.FullName));
            }

            return ParameterLessConstructorInfo.Invoke(null);
        }
    }
}
