using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Statistics
{
    public class AdditionalStatistic
    {
        public object Key { get; private set; }
        public Func<IObservable<double>, IObservable<object>> Calculate { get; private set; }

        public AdditionalStatistic(object key, Func<IObservable<double>, IObservable<object>> calculate)
        {
            this.Key = key;
            this.Calculate = calculate;
        }
    }
}
