using SquaredInfinity.Presentation.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace SquaredInfinity.Windows.MarkupExtensions
{
    public partial class MethodResultBinding : SmartBinding
    {
        public string MethodName { get; set; }
        public object Parameter { get; set; }
        public Binding ParameterBinding { get; set; }
        public Binding ReevaluateTriggerBinding { get; set; }

        public string NotifySourceChangedTriggerPropertyName { get; set; }

        public MethodResultBinding()
        {
            Mode = BindingMode.OneWay;
        }

        public MethodResultBinding(string methodNameOrFullSourcePath)
        {
            Mode = BindingMode.OneWay;

            var ix_last_dot = methodNameOrFullSourcePath.LastIndexOf('.');

            if (ix_last_dot < 0)
            {
                MethodName = methodNameOrFullSourcePath;
            }
            else
            {
                MethodName = methodNameOrFullSourcePath.Substring(ix_last_dot + 1);

                Source = methodNameOrFullSourcePath.Substring(0, ix_last_dot);
            }
        }

        protected override BindingBase InitialiseBinding(IServiceProvider serviceProvider)
        {
            var multiBinding = new MultiBinding();

            multiBinding.Mode = BindingMode.OneWay;

            var contextBinding = new Binding();
            UpdateBindingFromSource(contextBinding);
            multiBinding.Bindings.Add(contextBinding);

            int nonParameterBindingsCount = 1;

            if(NotifySourceChangedTriggerPropertyName != null)
            {
                var sourceChangedTriggerBinding = new Binding();
                UpdateBindingFromSource(sourceChangedTriggerBinding);

                var path = sourceChangedTriggerBinding.Path.Path;

                if (path != null && path.Length > 0)
                {
                    sourceChangedTriggerBinding.Path = new System.Windows.PropertyPath(path + "." + NotifySourceChangedTriggerPropertyName);
                }
                else
                {
                    sourceChangedTriggerBinding.Path = new System.Windows.PropertyPath(NotifySourceChangedTriggerPropertyName);
                }

                multiBinding.Bindings.Add(sourceChangedTriggerBinding);

                nonParameterBindingsCount++;
            }

            if (ReevaluateTriggerBinding != null)
            {
                UpdateBindingFromSource(ReevaluateTriggerBinding, "");
                multiBinding.Bindings.Add(ReevaluateTriggerBinding);
                nonParameterBindingsCount++;
            }

            if (ParameterBinding != null)
            {
                multiBinding.Bindings.Add(ParameterBinding);

                var converter = 
                    new MixedCompositeConverter(
                    new MethodWithParametersResultConverter(MethodName, nonParameterBindingsCount, hardParameters: null), 
                    Converter);

                converter.IgnoreUnsetValues = true;

                multiBinding.Converter = converter;
            }
            else
            {
                if (Parameter == null)
                {
                    var converter =
                        new MixedCompositeConverter(
                        new MethodWithParametersResultConverter(MethodName, nonParameterBindingsCount, hardParameters: null),
                        Converter);
                    converter.IgnoreUnsetValues = true;
                    multiBinding.Converter = converter;
                }
                else
                {
                    var converter =
                        new MixedCompositeConverter(
                        new MethodWithParametersResultConverter(MethodName, nonParameterBindingsCount, hardParameters: new object[] { Parameter }),
                        Converter);
                    converter.IgnoreUnsetValues = true;
                    multiBinding.Converter = converter;
                }
            }

            return multiBinding;
        }
    }
}
