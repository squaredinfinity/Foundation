using System;
using SquaredInfinity.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SquaredInfinity.Maths.Parsing.Terms
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class NumberSequenceTerm : Term
    {
        readonly static ColonOperator ColonOperator = new ColonOperator();
        readonly static ArrayParser ArrayParser = new ArrayParser();

        public string Input { get; set; }
        
        protected override object DoGetValue()
        {
            if (Input.IsNullOrEmpty())
                return new double[0];

            var result = (double[])null;

            if (ArrayParser.TryCreateArray(Input, ref result))
                return result;

            if (ColonOperator.TryCreateArray(Input, ref result))
                return result;

            return new double[0];
        }

        public string DebuggerDisplay
        {
            get { return $"{Input.ToStringWithNullOrEmpty()}"; }
        }
    }
}
