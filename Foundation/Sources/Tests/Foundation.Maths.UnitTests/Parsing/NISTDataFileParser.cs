using SquaredInfinity.Foundation.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Parsing
{
    public class NTST_UnivariateSummaryStatisticsTest
    {
        public double ExpectedSampleMean { get; set; }
        public double ExpectedSampleStdDev { get; set; }
        public IReadOnlyList<double> Data { get; set; }
    }

    public class NISTDataFileParser
    {
        string TestsNamespace = @"SquaredInfinity.Foundation.Maths.Statistics.Data.NIST";

        public NTST_UnivariateSummaryStatisticsTest GetUnivariateTest(string testName)
        {
            var result = new NTST_UnivariateSummaryStatisticsTest();

            using (var stream = ResourcesManager.LoadEmbeddedResourceFromThisAssembly($"{TestsNamespace}.Univariate_Summary_Statistics", $"{testName}.dat"))
            using (var sr = new StreamReader(stream))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if(line.StartsWith("Sample Mean"))
                    {
                        result.ExpectedSampleMean = GetCertifiedValue(line);
                        continue;
                    }

                    if(line.StartsWith("Sample Standard Deviation"))
                    {
                        result.ExpectedSampleStdDev = GetCertifiedValue(line);
                    }

                    if(line.StartsWith("---------"))
                    {
                        result.Data = GetData(sr);
                    }
                }
            }

            return result;
        }

        IReadOnlyList<double> GetData(StreamReader sr)
        {
            var result = new List<double>(capacity: 5000);

            while(!sr.EndOfStream)
            {
                var line = sr.ReadLine().Trim();

                var d = double.Parse(line, CultureInfo.InvariantCulture);

                result.Add(d);
            }

            result.TrimExcess();

            return result;
        }

        double GetCertifiedValue(string line)
        {
            var ix_of_colon = line.LastIndexOf(":");

            var dirty_value_as_string = line.Substring(ix_of_colon + 1).Trim();
            var value_as_string = dirty_value_as_string;

            var ix_of_space = dirty_value_as_string.IndexOf(' ');

            if(ix_of_space != -1)
                value_as_string = dirty_value_as_string.Substring(0, ix_of_space);
            



            

            var result = double.Parse(value_as_string, CultureInfo.InvariantCulture);

            return result;
        }
    }
}
