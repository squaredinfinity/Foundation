using SquaredInfinity.Foundation.Presentation.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public partial class MethodResultBinding : SmartBinding
    {
        public string MethodName { get; set; }
        public object Parameter { get; set; }
        public Binding ParameterBinding { get; set; }

        public MethodResultBinding()
        { }

        public MethodResultBinding(string methodName)
        {
            MethodName = methodName;
        }

        protected override BindingBase InitialiseBinding(IServiceProvider serviceProvider)
        {
            var multiBinding = new MultiBinding();

            var contextBinding = new Binding();
            UpdateBindingFromSource(contextBinding);
            contextBinding.Mode = BindingMode.OneTime;
            multiBinding.Bindings.Add(contextBinding);

            if (ParameterBinding != null)
            {
                multiBinding.Bindings.Add(ParameterBinding);
                multiBinding.Converter = new MixedCompositeConverter(new MethodWithParametersResultConverter(MethodName, null), Converter);
            }
            else
            {
                if (Parameter == null)
                {
                    multiBinding.Converter = new MixedCompositeConverter(new MethodWithParametersResultConverter(MethodName, null), Converter);
                }
                else
                {
                    multiBinding.Converter = new MixedCompositeConverter(new MethodWithParametersResultConverter(MethodName, new object[] { Parameter }), Converter);
                }
            }

            return multiBinding;
        }
    }
}
