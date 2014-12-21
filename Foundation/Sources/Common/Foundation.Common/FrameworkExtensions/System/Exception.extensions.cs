using SquaredInfinity.Foundation.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Tries to add the specified context data to the exception.
        /// It's is safer to call it than to add data directly to Exception.Data dictionary (guid key will be used to avoid duplicate keys).
        /// ! this method is private because TryAddContextData() should be used instead
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private static void AddContextData<T>(this Exception ex, string key, T value)
        {
            ExceptionContextData additionalExceptionData = new ExceptionContextData();

            additionalExceptionData.Key = key;
            additionalExceptionData.Value = value;

            StackTrace stackTrace = new StackTrace(1);
            var callerFrame = stackTrace.GetFrame(0);
            var callingMethod = callerFrame.GetMethod();
            additionalExceptionData.TargetSite = string.Format("{0}.{1}", callingMethod.DeclaringType.FullName, callingMethod.Name);

            // use Guid as a key to avoid duplicates.
            // the orginal key will be later extracted from AdditionalExceptionData
            ex.Data.Add(Guid.NewGuid(), additionalExceptionData);
        }

        /// <summary>
        /// Adds the specified context data to the exception.
        /// It's is safer to call it than to add data directly to Exception.Data dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <param name="key"></param>
        /// <param name="getValue"></param>
        public static bool TryAddContextData<T>(this Exception ex, string key, Func<T> getValue)
        {
            try
            {
                ex.AddContextData(key, getValue());
                return true;
            }
            catch (Exception newException)
            {
                ex.AddContextData(string.Format("Unable to add [{0}]", key), newException.ToString());
                return false;
            }
        }

    }
}
