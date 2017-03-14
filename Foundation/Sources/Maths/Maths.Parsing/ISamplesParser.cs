using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Parsing
{
    public interface ISamplesParser
    {
        IEnumerable<double> Parse(Stream input);
    }
}
