using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Parsing
{
    public abstract class SamplesParser : ISamplesParser
    {
        public IEnumerable<double> Parse(Stream input)
        {
            return DoParse(input);   
        }

        protected abstract IEnumerable<double> DoParse(Stream input);
    }
}
