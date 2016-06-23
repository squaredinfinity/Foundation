using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SquaredInfinity.Foundation.Maths.Parsing.Terms
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ArrayExpansion
    {
        readonly static ColonOperator ColonOperator = new ColonOperator();

        public string Input { get; set; }

        public double[] Expand()
        {
            if (Input.IsNullOrEmpty())
                return new double[0];

            var result = (double[])null;

            if(ColonOperator.TryCreateArray(Input, ref result))
            {
                return result;
            }
            
            return new double[0];
        }

        public string DebuggerDisplay
        {
            get { return $"{Input.ToStringWithNullOrEmpty()}"; }
        }
    }
}
