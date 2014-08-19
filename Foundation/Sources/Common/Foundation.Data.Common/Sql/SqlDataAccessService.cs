using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Xml.Linq;
using System.Data.SqlTypes;
using SquaredInfinity.Foundation.Diagnostics;

namespace SquaredInfinity.Foundation.Data.Sql
{
    /// <summary>
    /// Provides metods of accessing SQL Databases
    /// </summary>
    public class SqlDataAccessService : DataAccessService<SqlConnection, SqlCommand, SqlParameter, SqlDataReader>
    {
        public SqlDataAccessService(
            string connectionString, 
            TimeSpan defaultCommandTimeout)
            : this(null, new ConnectionFactory<SqlConnection>(connectionString), defaultCommandTimeout)
        { }

        public SqlDataAccessService(
            ILogger logger,
            ConnectionFactory<SqlConnection> connectionFactory, 
            TimeSpan defaultCommandTimeout)
            : base(logger, connectionFactory, defaultCommandTimeout)
        {
            RetryPolicy = new RetryPolicy();
            RetryPolicy.DefaultTransientFaultFilters.Add(new SqlTransientFaultFilter());
        }

        /// <summary>
        /// Creates a SqlParameter with given name and value.
        /// clr value will automatically be converted to DB value (e.g. null to DBNull)
        /// </summary>
        /// <param name="parameterName">name of SQL Parameter</param>
        /// <param name="clrValue">value of Sql Parameter</param>
        /// <returns></returns>
        public override SqlParameter CreateParameter(string parameterName, object clrValue)
        {
            var result = new SqlParameter();

            result.ParameterName = parameterName;

            //# null -> DB NULL
            if (clrValue == null)
            {
                result.Value = DBNull.Value;
                return result;
            }

            //# TimeSpan
            //  Sql Server does not support TimeSpan, convert it to number of ticks
            var is_ts = clrValue is TimeSpan;
            if (is_ts)
            {
                result.Value = ((TimeSpan) clrValue).Ticks;
                return result;
            }

            //# XDocument
            //  Sql Server does not support XDocument, convert it to xml string
            var xDocument = clrValue as XDocument;
            if (xDocument != null)
            {
                result.Value = xDocument.ToString(SaveOptions.None);
                return result;
            }

            //# XElement
            //  Sql Server does not support XElement, convert it to xml string
            var xElement = clrValue as XElement;
            if (xElement != null)
            {
                result.Value = xElement.ToString(SaveOptions.None);
                return result;
            }

            result.Value = clrValue;
            return result;
        }
    }
}
