using SquaredInfinity.Foundation.Diagnostics.ContextDataCollectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Diagnostics.TextTemplates;
using SquaredInfinity.Foundation.Diagnostics.TextTemplates.Substitutions;
using SquaredInfinity.Foundation.Diagnostics.TextTemplates.Placeholders;

namespace SquaredInfinity.Foundation.Diagnostics.Formatters
{
    public class PatternFormatter : IFormatter
    {
        static readonly Regex PatternPlaceholderRegex =
            new Regex(@"(?<!{){(?<content>[^{}]*)}(?!})", RegexOptions.Compiled);


        static readonly Regex ConditionalPlaceholderRegex =
            new Regex(@"{[?](?<not>[!]?)(?<condition>.*)=>(?<true>.*)}", RegexOptions.Compiled);

        string _pattern;
        public string Pattern
        {
            get { return _pattern; }
            set
            {
                _pattern = value;
                Initialize();
            }
        }

        public string Name { get; set; }

        public PatternFormatter()
        {
            Pattern = "{Event.Message}";

            TextTemplateProcessingServce = new TemplateProcessingService();
            TextTemplateProcessingServce.InternalContext.Functions.AddFunction(
                new Function(
                    "dump",
                    (originalValue, input, parameters) =>
                    {
                        return input.DumpToString();
                    }));
            //TextTemplateProcessingServce.InternalContext.Functions.AddFunction(
            //    new Function(
            //        "dumpHtml",
            //        (originalValue, input, parameters) =>
            //        {
            //            return input.DumpToHtml();
            //        }));

            TextTemplateProcessingServce.InternalContext.Functions.AddFunction(
                new Function(
                    "dumpWithHeader",
                    (originalValue, input, parameters) =>
                    {
                        throw new NotImplementedException();
                        //var kvp = new KeyValuePair(parameters.First().ToString().Trim(new char[] { '\'' }), input);

                        //// todo: apply descriptors
                        //var result = new StringObjectWriter().Write(kvp);

                        //return result;
                    }));

            // Event.Exception.DeepOriginHash - Type + TargetSite of top + all inner
            // Event.Exception.ShallowOriginHash - Type + TargetSite of top
            // Event.Exception.DeepStackTraceHash - Type + StackTrace of top + all inner
            // Event.Exception.ShallowStackTraceHash
            // Event.Exception.DeepHash - Type + Message + Stack Trace of top + all inner

            TextTemplateProcessingServce.InternalContext.Substitutions.AddOrUpdateSubstitution(
                new EvaluatedSubstitution("Event.DeepStackTraceHash",
                    (context) =>
                    {
                        var substitution = (Substitution)null;
                        if (!context.Substitutions.TryGetSubstitution("Event.Exception", out substitution))
                            return "";

                        var ex = substitution.EvaluateOutput(context) as Exception;

                        if (ex == null)
                            return "";

                        var result = ex.DeepStackTraceHash();

                        return result;
                    }));
        }



        TemplateProcessingService TextTemplateProcessingServce = new TemplateProcessingService();
        Template Template;

        public string Format(IDiagnosticEvent de)
        {
            var context = new TextTemplateProcessingContext();

            foreach (var property in de.Properties)
            {
                var prop = property;

                context.Substitutions.AddOrUpdateSubstitution(
                    property.UniqueName,
                    (cx) =>
                    {
                        return prop.Value;
                    });
            }

            var txt = TextTemplateProcessingServce.ProcessTemplate(Template, context);

            return txt.ExpandTabs();
        }

        void Initialize()
        {
            DataRequestCache.Clear();

            var lines = Pattern.GetLines(keepLineBreaks: false);
            var trimmedPattern = string.Concat(lines.Select(l => l.Trim()));

            Template = Template.FromText(trimmedPattern);

            var placeholders = Template.GetPlaceholders();

            for (int i = 0; i < placeholders.Count; i++)
            {
                var ph = placeholders[i];

                if (!ph.SubstitutionPattern.StartsWith("'") && !ph.SubstitutionPattern.EndsWith("'"))
                {
                    var dr = new DataRequest();
                    dr.Data = ph.SubstitutionPattern;
                    DataRequestCache.Add(dr);

                    var cph = ph as ConditionalPlaceholder;
                    if (cph != null)
                    {

                        if (!cph.TrueOutcomeSubstitutionPattern.StartsWith("'") && !cph.TrueOutcomeSubstitutionPattern.EndsWith("'"))
                        {
                            dr = new DataRequest();
                            dr.Data = cph.TrueOutcomeSubstitutionPattern;
                            DataRequestCache.Add(dr);
                        }
                    }
                }
            }
        }

        List<IDataRequest> DataRequestCache = new List<IDataRequest>();

        public  IReadOnlyList<IDataRequest> GetRequestedContextData()
        {
            return DataRequestCache;
        }
    }
}
