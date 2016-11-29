using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Diagnostics;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class ExceptionExtensions
    {
        internal static string DeepStackTraceHash(this Exception ex)
        {
            var sb = new StringBuilder();

            while (ex != null)
            {
                sb.AppendLine($"{ex.GetType().FullName}+{ex.StackTrace}");

                var aggregate = ex as AggregateException;

                if (aggregate != null)
                {
                    foreach (var cEx in aggregate.InnerExceptions)
                    {
                        sb.AppendLine(cEx.DeepStackTraceHash());
                    }

                    ex = null;
                }
                else
                {
                    ex = ex.InnerException;
                }
            }

            using (MD5 md5 = MD5.Create())
            {
                var data = md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));

                var hashSB = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    hashSB.Append(data[i].ToString("x2"));
                }

                return hashSB.ToString();
            }
        }
    }
}
