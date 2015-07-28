using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.TextTemplates.Substitutions
{
    // TODO: make it a dictionary, so that input can be a key
    public class SubstitutionCollection : IEnumerable<Substitution>
    {
        Dictionary<string, Substitution> InternalStorage = new Dictionary<string, Substitution>(27);

        public void AddOrUpdateSubstitution(Substitution substitution)
        {
            if (InternalStorage.ContainsKey(substitution.Input))
            {
                InternalStorage[substitution.Input] = substitution;
                return;
            }

            InternalStorage.Add(substitution.Input, substitution);
        }

        public void AddOrUpdateSubstitution(string input, object output)
        {
            if (InternalStorage.ContainsKey(input))
            {
                InternalStorage[input] = new TextSubstitution(input, output);
                return;
            }

            InternalStorage.Add(input, new TextSubstitution(input, output));
        }

        public void AddOrUpdateSubstitution(string input, Func<TextTemplateProcessingContext, object> evaluateOutput)
        {
            if (InternalStorage.ContainsKey(input))
            {
                InternalStorage[input] = new EvaluatedSubstitution(input, evaluateOutput);
                return;
            }

            InternalStorage.Add(input, new EvaluatedSubstitution(input, evaluateOutput));
        }

        public bool TryGetSubstitution(string name, out Substitution substitution)
        {
            substitution = null;

            if (!InternalStorage.ContainsKey(name))
                return false;

            substitution = InternalStorage[name];
            return true;
        }

        public IEnumerator<Substitution> GetEnumerator()
        {
            return InternalStorage.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return InternalStorage.GetEnumerator();
        }
    }
}
