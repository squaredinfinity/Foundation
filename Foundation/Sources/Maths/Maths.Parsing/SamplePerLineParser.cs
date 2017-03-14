using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Parsing
{
    /// <summary>
    /// Parses samples provided in a form of one sample per line
    /// </summary>
    public class SamplePerLineParser : SamplesParser
    {
        protected override IEnumerable<double> DoParse(Stream input)
        {
            using (var sr = new StreamReader(input))
            {
                while(!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    var sample = double.NaN;

                    if(double.TryParse(line, out sample))
                    {
                        yield return sample;
                    }
                }
            }
        }
    }
}
