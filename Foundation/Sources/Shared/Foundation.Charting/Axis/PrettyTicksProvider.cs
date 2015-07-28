using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Charting.Axis
{
    public class PrettyTicksProvider
    {
        public void GetTicks(double min, double max, int numberOfTicks)
        {
            var range = NiceNumber(max - min, shouldRound: false);
            //var range = max - min;

            var tick_spacing = NiceNumber(range / (numberOfTicks - 1), shouldRound: true);

            var new_min = Math.Floor(min / tick_spacing) * tick_spacing;
            var new_max = Math.Ceiling(max / tick_spacing) * tick_spacing;

            var fractional_digits_to_show = Math.Max(Math.Floor(Math.Log10(tick_spacing)), 0);

            var end = new_max + (tick_spacing * .5);
            for(var x = new_min; x <= end; x+=tick_spacing)
            {
                Trace.WriteLine(x);
            }
        }

        public double NiceNumber(double x, bool shouldRound)
        {
            var exp = Math.Floor(Math.Log10(x));

            var fractional_part = x / Math.Pow(10, exp);

            var result = 0.0;

            if(shouldRound)
            {
                if (fractional_part < 1.5)
                    result = 1.0;
                else if (fractional_part < 3)
                    result = 2.0;
                else if (fractional_part < 7)
                    result = 5.0;
                else
                    result = 10.0;
            }
            else
            {
                if (fractional_part <= 1)
                    result = 1.0;
                else if (fractional_part <= 2)
                    result = 2.0;
                else if (fractional_part <= 5)
                    result = 5.0;
                else
                    result = 10.0;
            }

            return result * Math.Pow(10, exp);
        }
    }
}
