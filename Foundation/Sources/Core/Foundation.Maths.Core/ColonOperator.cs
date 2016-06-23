using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths
{
    public class ColonOperator
    {
        /// <summary>
        /// j:k produces array [j j+1, j+2, .., j+m], where m = fix(k-j)
        /// when j and k are both integers, the array is [j, j+1, j+2, ..., k]
        /// resulting array has m + 1 elements
        /// </summary>
        public double[] CreateArray(double j, double k)
        {
            var m = (int)(k - j).Fix();

            var result = new double[m + 1];

            for(int i = 0; i <= m; i++)
            {
                result[i] = j + i;
            }

            return result;
        }

        /// <summary>
        /// j:i:k produces array [j, j+i, j+2i, .., j+mi], where m = fix((k-j)/i)
        /// </summary>
        public double[] CreateArray(double j, double i, double k)
        {
            var m = (int)((k - j)/i).Fix();

            var result = new double[m + 1];

            for (int ix = 0; ix <= m; ix++)
            {
                result[ix] = j + ix * i;
            }

            return result;
        }

        public bool TryCreateArray(string input, ref double[] result)
        {
            var terms = input.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if(terms.Length == 2)
            {
                var j = double.Parse(terms[0]);
                var k = double.Parse(terms[1]);

                
                result = CreateArray(j, k);
                return true;
            }
            else
            if(terms.Length == 3)
            {
                var j = double.Parse(terms[0]);
                var i = double.Parse(terms[1]);
                var k = double.Parse(terms[2]);

                result = CreateArray(j, i, k);
                return true;
            }

            result = null;
            return false;
        }
    }
}
