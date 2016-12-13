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
        readonly static ColonOperator ColonOperator = new ColonOperator();

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
                {
                    var expanded_array = (double[])null;

                    if (ColonOperator.TryCreateArray(t, ref expanded_array))
                    {
                        numbers.AddRange(expanded_array);
                    }
                    else
                    {
                        // parsing failed, at the moment default action is to just continue, 
                        // but maybe optionally exception should be thrown instead?
                        continue;
                    }
                }
            }

            result = numbers.ToArray();
            return true;
        }
    }
}
