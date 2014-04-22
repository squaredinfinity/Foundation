using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    /// <summary>
    /// Provides additional data for the exception.
    /// It will be stored securely inside exception object.
    /// </summary>
    [Serializable] // serializable attribute required for it to be stored inside exception object
    public class ExceptionContextData
    {
        public string Key { get; internal set; }
        public object Value { get; internal set; }
        public string TargetSite { get; internal set; }
    } 
}
