using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class ExceptionExtensions
    {
        ///// <summary>
        ///// Tries to add the specified context data to the exception.
        ///// It's is safer to call it than to add data directly to Exception.Data dictionary (guid key will be used to avoid duplicate keys).
        ///// ! this method is private because TryAddContextData() should be used instead
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="ex"></param>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        static void AddContextData<T>(this Exception ex, string key, T value, string callerMemberName = "")
        {
            ExceptionContextData additionalExceptionData = new ExceptionContextData();

            additionalExceptionData.Key = key;
            additionalExceptionData.Value = value;
            additionalExceptionData.CallerMember = callerMemberName;

            // only ever called from Try Add Context Data
            StackTrace stackTrace = new StackTrace(2);

            if (stackTrace != null)
            {
                var callerFrame = stackTrace.GetFrame(0);

                if (callerFrame != null)
                {
                    var callingMethod = callerFrame.GetMethod();

                    if (callingMethod != null)
                    {
                        additionalExceptionData.TargetSite = string.Format("{0}.{1}", callingMethod.DeclaringType.FullName, callingMethod.Name);
                    }
                }
            }

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
        /// <param name="getData"></param>
        public static bool TryAddContextData<T>(this Exception ex, string key, Func<T> getData, [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                if (ex.Data == null)
                    return false;

                ex.AddContextData(key, getData(), callerMemberName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Adds the specified context data to the exception.
        /// It's is safer to call it than to add data directly to Exception.Data dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <param name="key"></param>
        /// <param name="getData"></param>
        public static bool TryAddContextData<T>(this Exception ex, string key, T data, [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                if (ex.Data == null)
                    return false;

                ex.AddContextData(key, data, callerMemberName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
