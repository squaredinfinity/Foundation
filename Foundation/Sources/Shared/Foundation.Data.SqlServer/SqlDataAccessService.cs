using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using System.Xml.Linq;
using System.Data.SqlTypes;
using System.Data;
using System.Collections;
using System.Data.Common;
using Microsoft.SqlServer.Server;
using SquaredInfinity.Diagnostics;

namespace SquaredInfinity.Data.SqlServer
{
    /// <summary>
    /// Provides metods of accessing SQL Databases
    /// </summary>
    public class SqlDataAccessService : DataAccessService<SqlConnection, SqlCommand, SqlParameter, SqlDataReader>
    {
        public SqlDataAccessService(
            string connectionString)
            : this(null, new ConnectionFactory<SqlConnection>(connectionString), TimeSpan.FromSeconds(30))
        { }

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
            var retry_policy_options = new RetryPolicyOptions();
            retry_policy_options.TransientFaultFilters.Add(new SqlTransientFaultFilter());

            RetryPolicy = new RetryPolicy(retry_policy_options);
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

        public SqlParameter CreateOutParameter(string parameterName, SqlDbType  type)
        {
            var result = new SqlParameter();
            result.ParameterName = parameterName;
            result.SqlDbType = type;
            result.Direction = System.Data.ParameterDirection.Output;

            return result;
        }

        public SqlParameter CreateTableParameter(string parameterName, IEnumerable source, string columnName, SqlDbType columnType)
        {
            var result = new SqlParameter();
            result.ParameterName = parameterName;
            result.SqlDbType = SqlDbType.Structured;

            var md = new SqlMetaData[1];
            md[0] = new SqlMetaData(columnName, columnType);

            // evaluate the source
            // if it yelds no values, then value should be set to null
            // otherwise use proper value
            // this is a requirement of SQL Server, passing empty enumerable as value of Table Parameter will result in exception being thrown

            var value = new SqlDataRecordEnumerableWrapper(source, md).ToArray();

            if (value.Length == 0)
                result.Value = null;
            else
                result.Value = value;

           return result;
        }

        public override SqlParameter CreateOutParameter(string parameterName, DbType type)
        {
            var result = new SqlParameter();
            result.ParameterName = parameterName;
            result.DbType = type;
            result.Direction = System.Data.ParameterDirection.Output;

            return result;
        }
    }
}
