using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Data.SqlServer
{
    public class SqlTransientFaultFilter : ITransientFaultFilter
    {
        readonly List<int> TransientErrorNumbers = new List<int>();
         
        public SqlTransientFaultFilter()
        {
            // default transient errors taken from:
            // http://social.technet.microsoft.com/wiki/contents/articles/1541.windows-azure-sql-database-connection-management.aspx
            // http://msdn.microsoft.com/en-us/library/ff394106.aspx

            TransientErrorNumbers.Add(64);
            // A connection was successfully established with the server, 
            // but then an error occurred during the login process. 
            // (provider: TCP Provider, error: 0 - The specified network name is no longer available.)

            TransientErrorNumbers.Add(233);
            // http://technet.microsoft.com/en-us/library/bb326280.aspx
            // A connection was successfully established with the server, 
            // but then an error occurred during the login process. 
            // (provider: Shared Memory Provider, error: 0 - No process is on the other end of the pipe.) (Microsoft SQL Server, Error: 233)

            TransientErrorNumbers.Add(40197);
            // The service has encountered an error processing your request. Please try again. Error code %d.
            // You will receive this error, when the service is down due to software or hardware upgrades, 
            // hardware failures, or any other failover problems. 
            // The error code (%d) embedded within the message of error 40197 provides additional 
            // information about the kind of failure or failover that occurred. 
            // Some examples of the error codes embedded within the message of error 40197 are 40020, 40143, 40166, and 40540.
            // Reconnecting to your SQL Database server will automatically connect you to a healthy copy of your database. 
            // Your application must catch error 40197, log the embedded error code (%d) within the message for troubleshooting,
            // and try reconnecting to SQL Database until the resources are available, and your connection is established again.

            TransientErrorNumbers.Add(40501);
            // The service is currently busy. Retry the request after 10 seconds. Incident ID: %ls. Code: %d.

            TransientErrorNumbers.Add(10053);
            TransientErrorNumbers.Add(10054);
            // A transport-level error has occurred when receiving results from the server. An established connection was aborted by the software in your host machine

            TransientErrorNumbers.Add(10060);
            // http://technet.microsoft.com/en-us/library/bb326282.aspx
            // An error has occurred while establishing a connection to the server. 
            // When connecting to SQL Server, this failure may be caused by the fact that under the default settings 
            // SQL Server does not allow remote connections. 
            // (provider: TCP Provider, error: 0 - A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond.) 
            // (Microsoft SQL Server, Error: 10060)

            TransientErrorNumbers.Add(40613);
            // Database '%.*ls' on server '%.*ls' is not currently available. Please retry the connection later. 
            // If the problem persists, contact customer support, and provide them the session tracing ID of '%.*ls'.
            
            TransientErrorNumbers.Add(40553);
            // The session has been terminated because of excessive memory usage. Try modifying your query to process fewer rows.

            // NOTE: NOT INCLUDED, SEE NOTES BELOW
            //x TransientErrorNumbers.Add(40143);
            //x TransientErrorNumbers.Add(40166);
            // Note: You may see error codes 40143 and 40166 embedded within the message of error 40197. 
            // The error codes 40143 and 40166 provide additional information about the kind of failover that occurred. 
            // Do not modify your application to catch error codes 40143 and 40166. 
            // Your application should catch 40197 and try reconnecting to SQL Database 
            // until the resources are available and your connection is established again.
        }

        /// <summary>
        /// Checks if exception should be classified as a transient fault
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>True if exception is a transient fault, False otherwise</returns>
        public bool IsTransientFault(Exception ex)
        {
            var result = false;

            var sql = ex as SqlException;

            if(sql != null)
            {
                result = IsSqlExceptionTransient(sql);
            }

            if (result == true)
                return true;

            return IsExceptionTransient(ex);
        }

        /// <summary>
        /// Checks if exception should be classified as a transient fault.
        /// </summary>
        /// <param name="ex">An exception (this will never be a SqlException, those are handled by IsSqlExceptionTransient)</param>
        /// <returns>True if exception is a transient fault, False otherwise</returns>
        protected virtual bool IsExceptionTransient(Exception ex)
        {
            return (ex is TimeoutException);
        }

        /// <summary>
        /// Checks if Sql Exception should be classified as a transient fault.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>True if exception is a transient fault, False otherwise</returns>
        protected virtual bool IsSqlExceptionTransient(SqlException ex)
        {
            return ex.Number.IsIn(TransientErrorNumbers);
        }
    }
}
