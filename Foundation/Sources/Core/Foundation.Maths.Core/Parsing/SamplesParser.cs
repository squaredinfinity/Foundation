using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Parsing
{
    public abstract class SamplesParser : ISamplesParser
    {
        public IEnumerable<double> ParseSamples(Stream input)
        {
            return DoParseSamples(input);   
        }

        protected abstract IEnumerable<double> DoParseSamples(Stream input);
    }
}
