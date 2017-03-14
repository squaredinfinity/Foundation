using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using SquaredInfinity.Types.Description.Reflection;
using System.Reflection.Emit;
using SquaredInfinity.ILGeneration;

namespace SquaredInfinity.Types.Description.IL
{
    public partial class ILBasedTypeDescription
    {
        ReflectionBasedTypeDescription Source;

        ConstructorInvocation CreateInstanceDelegate;

        public ILBasedTypeDescription(ReflectionBasedTypeDescription source)
        {
            this.Source = source;

            if(source.ParameterLessConstructorInfo != null)
                this.CreateInstanceDelegate = source.ParameterLessConstructorInfo.EmitConstructorInvocationDelegate();
            else if(source.Type.IsValueType || source.Type.IsEnum)
                this.CreateInstanceDelegate = source.Type.EmitConstructorInvocationDelegate();
        }        
    }
}
