using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Diagnostics.TextTemplates.Substitutions;

namespace SquaredInfinity.Foundation.Diagnostics.TextTemplates.Placeholders
{
    [DebuggerDisplay("{RawValue}")]
    public class Placeholder
    {
        readonly static ILogger Diagnostics = InternalLogger.CreateLoggerForType<Placeholder>();

        public string RawValue { get; set; }

        public int StartIndex { get; set; }

        public int Length { get; set; }

        public string SubstitutionPattern { get; set; }

        public List<FunctionDefinition> Functions { get; set; }

        public Placeholder()
        {
            Functions = new List<FunctionDefinition>();
        }

        public virtual bool TryEvaluate(TextTemplateProcessingContext cx, out object result)
        {
            return TryEvaluateInternal(cx, SubstitutionPattern, Functions, out result);
        }

        public static bool TryEvaluateInternal(
            TextTemplateProcessingContext cx,
            string substitutionPattern,

#if UPTO_DOTNET40
        IList<FunctionDefinition>
#else
 IReadOnlyList<FunctionDefinition>
#endif
 functions,
            out object result)
        {
            result = (object)null;
            var initialValue = (object)null;

            //# try Variable
            if (cx.Variables.ContainsKey(substitutionPattern))
            {
                result = cx.Variables[substitutionPattern];
                initialValue = result;
            }
            else // # try Substitutions
            {
                Substitution subs = null;

                if (cx.Substitutions.TryGetSubstitution(substitutionPattern, out subs))
                {
                    result = subs.EvaluateOutput(cx);
                    initialValue = result;
                }
                else
                {
                    if (functions.Count == 0)
                    {
                        Diagnostics.Information(() => "No substitution found for pattern '{0}'".FormatWith(substitutionPattern));
                        return false;
                    }
                }
            }

            //# apply processing instructions

            for (var i = 0; i < functions.Count; i++)
            {
                var piDef = functions[i];

                Function inst = null;

                if (cx.Functions.TryGetFunction(piDef.Name, out inst))
                {
                    result = inst.Process(initialValue, result, piDef.Parameters);
                }
                else
                {
                    string formattedResult = null;
                    if (TryFormat(result, piDef.Name, out formattedResult))
                    {
                        result = formattedResult;
                    }
                    else
                    {
                        Diagnostics.Information(() => "Failed to process function {0}".FormatWith(piDef.Name));
                        return false;
                    }
                }
            }

            return true;
        }

        static bool TryFormat(object input, string format, out string result)
        {
            result = null;

            if (input is DateTime)
            {
                var dateTime = (DateTime)input;
                if (dateTime != null)
                {
                    result = dateTime.ToString(format);
                    return true;
                }
            }

            return false;
        }
    }
}
