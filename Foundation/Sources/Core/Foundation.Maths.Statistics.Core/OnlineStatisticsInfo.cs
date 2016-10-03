using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    public class OnlineStatisticsInfo
    {
        public UInt64 Count { get; internal set; }
        public double Mean { get; internal set; }
        public RangeInfo Range { get; internal set; }
        public double Min { get { return Range.Min; } }
        public double Max { get { return Range.Max; } }
        public VarianceInfo Variance { get; internal set; }
        public StdDevInfo StdDev { get; internal set; }

        readonly internal Dictionary<object, object> AdditionalStatisticsResults = new Dictionary<object, object>();
        public T GetValue<T>(object key)
        {
            return (T)AdditionalStatisticsResults[key];
        }
    }
}
