using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Extensions
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
