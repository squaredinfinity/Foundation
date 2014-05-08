using SquaredInfinity.Foundation.Diagnostics.TextTemplates.Placeholders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Diagnostics.TextTemplates
{
    public class Template
    {
        readonly static ILogger Diagnostics = InternalDiagnosticLogger.CreateLoggerForType<Template>();

        static readonly Regex PlaceholderRegex =
           new Regex(@"(?<!{){(?<content>[^{}]*)}(?!})", RegexOptions.Compiled);

        static readonly Regex ConditionalPlaceholderRegex =
            new Regex(@"{[?](?<not>[!]?)(?<condition>.*)?=>(?<true_outcome>.*)}", RegexOptions.Compiled);

        static readonly Regex FunctionsSplitRegex =
            new Regex(@"(?<!\\):", RegexOptions.Compiled);

        static readonly Regex FunctionRegex =
            new Regex(@"(?<name>.*)\((?<parameters>.*)\)", RegexOptions.Compiled);

        public string RawPattern { get; private set; }

        List<Placeholder> Placeholders { get; set; }

        public ReadOnlyCollection<Placeholder> GetPlaceholders()
        {
            return Placeholders.ToReadOnly();
        }

        public static Template FromText(string pattern)
        {
            var template = new Template();

            template.RawPattern = pattern;
            template.Placeholders = GetPlaceholdersFromPattern(pattern);

            return template;
        }

        static internal List<Placeholder> GetPlaceholdersFromPattern(string pattern)
        {
            var result = new List<Placeholder>();

            //# Find placeholders in pattern
            foreach (var patternMatch in PlaceholderRegex.Matches(pattern).Cast<Match>())
            {
                if (!patternMatch.Success)
                    continue;

                //# check if is conditional placeholder
                var conditionalMatch = ConditionalPlaceholderRegex.Match(patternMatch.Value);

                Placeholder placeholder = null;
                string[] contentAndInstructions = null;

                if (conditionalMatch.Success)
                {
                    var notGroup = conditionalMatch.Groups["not"];
                    var conditionGroup = conditionalMatch.Groups["condition"];
                    var trueGroup = conditionalMatch.Groups["true_outcome"];

                    placeholder = new ConditionalPlaceholder();

                    placeholder.RawValue = patternMatch.Value;
                    placeholder.StartIndex = patternMatch.Index;
                    placeholder.Length = patternMatch.Length;

                    contentAndInstructions = FunctionsSplitRegex.Split(conditionGroup.Value);

                    string substitutionPattern = string.Empty;
                    List<FunctionDefinition> functions = new List<FunctionDefinition>();

                    ProcessContentAndInstructions(
                        contentAndInstructions,
                        out substitutionPattern,
                        out functions);

                    placeholder.SubstitutionPattern = substitutionPattern.Replace(@"\:", ":");
                    placeholder.Functions = functions;

                    var trueOutcomeContentAndInstructions = FunctionsSplitRegex.Split(trueGroup.Value);

                    ProcessContentAndInstructions(
                        trueOutcomeContentAndInstructions,
                        out substitutionPattern,
                        out functions);

                    ((ConditionalPlaceholder)placeholder).TrueOutcomeSubstitutionPattern = substitutionPattern.Replace(@"\:", ":");
                    ((ConditionalPlaceholder)placeholder).TrueOutcomeProcessFunctions = functions;
                }
                else
                {
                    var contentGroup = patternMatch.Groups["content"];

                    if (!contentGroup.Success)
                    {
                        Diagnostics.Warning(() => "Unable to parse pattern: " + patternMatch.Value);
                        continue;
                    }

                    placeholder = new Placeholder();

                    placeholder.RawValue = patternMatch.Value;
                    placeholder.StartIndex = patternMatch.Index;
                    placeholder.Length = patternMatch.Length;

                    contentAndInstructions = FunctionsSplitRegex.Split(contentGroup.Value);

                    string substitutionPattern = string.Empty;
                    List<FunctionDefinition> functions = new List<FunctionDefinition>();

                    ProcessContentAndInstructions(
                        contentAndInstructions,
                        out substitutionPattern,
                        out functions);

                    placeholder.SubstitutionPattern = substitutionPattern;
                    placeholder.Functions = functions;
                }

                result.Add(placeholder);
            }

            return result;
        }

        static void ProcessContentAndInstructions(
            string[] contentAndInstructions,
            out string substitutionPattern,
            out List<FunctionDefinition> functions)
        {
            substitutionPattern = contentAndInstructions[0];

            functions = new List<FunctionDefinition>();

            if (contentAndInstructions.Length > 1)
            {
                for (int i = 1; i < contentAndInstructions.Length; i++)
                {
                    var contentAndInstruction = contentAndInstructions[i];

                    var match = FunctionRegex.Match(contentAndInstruction);

                    if (!match.Success)
                    {
                        // treat it as a formatting
                        Diagnostics.Information(
                            () =>
                                "Unable to find meaning of Content and Instruction {0}. Assuming that this is format specification."
                                .FormatWith(contentAndInstruction));

                        var formatFuncDef = new FunctionDefinition();
                        formatFuncDef.Name = "format";
                        formatFuncDef.Parameters.Add(contentAndInstruction);

                        functions.Add(formatFuncDef);

                        continue;
                    }

                    var nameGroup = match.Groups["name"];
                    var parametersGroup = match.Groups["parameters"];

                    var funcDef = new FunctionDefinition();
                    funcDef.Name = nameGroup.Value.ToLower();
                    funcDef.Parameters.AddRange(
                        parametersGroup.Value.Split(
                        new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    functions.Add(funcDef);
                }
            }
            else
            {
                if (substitutionPattern.StartsWith("'") && substitutionPattern.EndsWith("'"))
                {
                    var literalStringFuncDef = new FunctionDefinition();
                    literalStringFuncDef.Name = "literal";
                    literalStringFuncDef.Parameters.Add(substitutionPattern.Trim(new char[] { '\'' }).Replace("\\:", ":"));

                    functions.Add(literalStringFuncDef);
                }
            }
        }
    }
}
