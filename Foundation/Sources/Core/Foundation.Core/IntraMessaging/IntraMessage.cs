using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.IntraMessaging
{
    public class IntraMessage : IIntraMessage
    {
        public string UniqueName { get; set; }
        public object Payload { get; set; }

        #region Time Stamp

        const string TIMESTAMP = "__TIMESTAMP__";
        public DateTime TimeStamp
        {
            get
            {
                var result = DateTime.MinValue;

                Properties.IfContainsKey(TIMESTAMP, x => result = (DateTime)x);

                return result;
            }

            set { Properties[TIMESTAMP] = value; }
        }
        
        #endregion

        #region Is Synchronous

        const string ISSYNCHRONOUS = "__ISSYNCHRONOUS__";
        public bool IsSynchronous
        {
            get
            {
                var result = false;

                Properties.IfContainsKey(ISSYNCHRONOUS, x => result = (bool)x);

                return result;
            }

            set { Properties[ISSYNCHRONOUS] = value; }
        }

        #endregion

        public IntraMessagePropertyCollection Properties { get; private set; } = new IntraMessagePropertyCollection();
    }
}
