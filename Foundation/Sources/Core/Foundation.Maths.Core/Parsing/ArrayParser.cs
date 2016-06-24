using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Parsing
{
    public class ArrayParser
    {
        public bool TryCreateArray(string input, ref double[] result)
        {
            result = null;

            if (input.IsNullOrEmpty())
                return false;

            var terms = input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (terms.Length <= 0)
                return false;

            var numbers = new List<double>(capacity: terms.Length);

            foreach (var t in terms)
            {
                var n = 0.0;

                if (double.TryParse(t, out n))
                    numbers.Add(n);
                else
                    return false;
            }

            result = numbers.ToArray();
            return true;
        }
    }
}
