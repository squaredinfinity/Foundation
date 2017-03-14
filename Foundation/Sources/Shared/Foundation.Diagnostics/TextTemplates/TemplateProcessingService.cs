using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Diagnostics.TextTemplates
{
    public class TemplateProcessingService
    {
        public TextTemplateProcessingContext InternalContext { get; private set; }

        public TemplateProcessingService()
        {
            InternalContext = new TextTemplateProcessingContext();

            InitializeDefaultFunctions();
            InitializeDefaultSubstitutions();
        }

        protected virtual void InitializeDefaultSubstitutions()
        {
            InternalContext.Substitutions.AddOrUpdateSubstitution("Environment.CurrentDirectory",
                (cx) => Environment.CurrentDirectory);

            InternalContext.Substitutions.AddOrUpdateSubstitution("AppDomain.CurrentDomain.BaseDirectory",
                (cx) => AppDomain.CurrentDomain.BaseDirectory);

            InternalContext.Substitutions.AddOrUpdateSubstitution("DateTime.Now", (cx) => DateTime.Now);
            InternalContext.Substitutions.AddOrUpdateSubstitution("DateTime.UtcNow", (cx) => DateTime.UtcNow);

            InternalContext.Substitutions.AddOrUpdateSubstitution("NewGuid", (cx) => Guid.NewGuid());

            InternalContext.Substitutions.AddOrUpdateSubstitution("NewLine", Environment.NewLine);
            InternalContext.Substitutions.AddOrUpdateSubstitution("Tab", "\t");
            InternalContext.Substitutions.AddOrUpdateSubstitution("Space", " ");
        }

        protected virtual void InitializeDefaultFunctions()
        {
            InternalContext.Functions.AddFunction(
                new Function(
                    "Format",
                    new Function.FunctionDelegate((initial, input, parameters) =>
                    {
                        if (input is DateTime)
                        {
                            var dateTime = (DateTime)input;
                            return dateTime.ToString(parameters.First().ToString());
                        }

                        return input;
                    })));

            InternalContext.Functions.AddFunction(
                new Function(
                    "Literal",
                    new Function.FunctionDelegate((initial, input, parameters) =>
                    {
                        return parameters.First().ToString();
                    })));

            InternalContext.Functions.AddFunction(
                new Function(
                    "ToValidFileName",
                    new Function.FunctionDelegate((initial, input, parameters) =>
                    {
                        if (input is string)
                        {
                            var str = (string)input;

                            var invalidCharReplacement = "-";

                            if (parameters.Any())
                            {
                                invalidCharReplacement = parameters.First().ToString();
                            }

                            return str.ToValidFileName();
                        }

                        return input;
                    })));
        }

        public string ProcessTemplate(string templateText, TextTemplateProcessingContext context = null)
        {
            var template = Template.FromText(templateText);

            return ProcessTemplate(template, context);
        }

        public string ProcessTemplate(Template template, TextTemplateProcessingContext context = null)
        {
            if (context != null)
            {
                // make sure provided context has access to all default substitutions
                foreach (var sub in InternalContext.Substitutions)
                {
                    context.Substitutions.AddOrUpdateSubstitution(sub);
                }

                // make sure provided context has access to all default functions
                foreach (var function in InternalContext.Functions)
                {
                    if (!context.Functions.Contains(function))
                        context.Functions.AddFunction(function);
                }
            }

            var sb = new StringBuilder(template.RawPattern);

            var placeholders = template.GetPlaceholders();

            for (int i = placeholders.Count - 1; i >= 0; i--)
            {
                var ph = placeholders[i];

                object value = null;

                if (context != null && ph.TryEvaluate(context, out value))
                {
                    sb.Replace(ph.RawValue, value.ToString(valueWhenNull: "[NULL]", valueWhenEmpty: ""), ph.StartIndex, ph.Length);
                }
                else if (ph.TryEvaluate(InternalContext, out value))
                {
                    sb.Replace(ph.RawValue, value.ToString(valueWhenNull: "[NULL]", valueWhenEmpty: ""), ph.StartIndex, ph.Length);
                }
                else
                {
                    InternalTrace.Warning(() => $"Unable to process placeholder '{ph.RawValue}'");
                }
            }

            return sb.ToString();
        }
    }
}
