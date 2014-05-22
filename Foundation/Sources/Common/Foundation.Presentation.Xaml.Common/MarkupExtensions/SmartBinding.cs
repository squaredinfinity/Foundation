using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.MarkupExtensions
{
    public abstract partial class SmartBinding : MarkupExtension
    {
        static Regex SourceElementBindingRegex = new Regex("@(?<ElementName>[^.]+).{0,1}(?<Path>.*)", RegexOptions.Compiled);

        BindingBase InternalBinding { get; set; }

        public string Source { get; set; }
        public IValueConverter Converter { get; set; }

        public object ConverterParameter { get; set; }

        public int Delay { get; set; }

        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }

        protected void UpdateBindingFromSource(Binding binding)
        {
            UpdateBindingFromSource(binding, Source);

            binding.Delay = Delay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger;
        }

        protected static void UpdateBindingFromSource(Binding binding, string source)
        {
            if (!source.IsNullOrEmpty())
            {
                if (source.StartsWith("@"))
                {
                    if (source.StartsWith("@TemplatedParent"))
                    {
                        binding.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);

                        var ix_dot = source.IndexOf(".");

                        if(ix_dot > 0)
                        {
                            binding.Path = new PropertyPath(source.Substring(ix_dot + 1));
                        }
                    }
                    else if(source.StartsWith("@Self"))
                    {
                        binding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);

                        var ix_dot = source.IndexOf(".");

                        if (ix_dot > 0)
                        {
                            binding.Path = new PropertyPath(source.Substring(ix_dot + 1));
                        }
                    }
                    else
                    {
                        var match = SourceElementBindingRegex.Match(source);

                        var elementNameGroup = match.Groups["ElementName"];
                        var pathGroup = match.Groups["Path"];

                        binding.ElementName = elementNameGroup.Value;

                        if (pathGroup.Success)
                            binding.Path = new PropertyPath(pathGroup.Value);
                    }
                }
                else
                {
                    binding.Path = new PropertyPath(source);
                }
            }
        }

        protected abstract BindingBase InitialiseBinding(IServiceProvider serviceProvider);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // return if in design time
            if (!(serviceProvider is IXamlTypeResolver))
                return null;

            if (InternalBinding == null)
            {
                InternalBinding = InitialiseBinding(serviceProvider);
            }

            if (InternalBinding == null)
            {
                Debug.Fail("InternalBinding should never be null");
                return DependencyProperty.UnsetValue;
            }

            return InternalBinding.ProvideValue(serviceProvider);
        }
    }
}
