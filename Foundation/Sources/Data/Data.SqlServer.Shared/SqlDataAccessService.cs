using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Data.SqlTypes;
using System.Data;
using System.Collections;
using System.Data.Common;
using Microsoft.SqlServer.Server;

namespace SquaredInfinity.Data.SqlServer
{
    /// <summary>
    /// Provides metods of accessing SQL Databases
    /// </summary>
    public class SqlDataAccessService : DataAccessService<SqlConnection, SqlCommand, SqlParameter, SqlDataReader>, ISqlDataAccessService
    {
        #region Default Retry Policy

        static IRetryPolicy _defaultRetryPolicy;
        public static IRetryPolicy DefaultRetryPolicy
        {
            get { return _defaultRetryPolicy; }
            set
            {
                if (value == null)
                    throw new ArgumentException($"nameof(DefaultRetryPolicy) cannot be null.");

                _defaultRetryPolicy = value;
            }
        }

        #endregion

        public static TimeSpan DefaultTimeout { get; set; }

        #region Constructors

        static SqlDataAccessService()
        {
            DefaultTimeout = TimeSpan.FromSeconds(30);

            var retry_policy_options = new RetryPolicyOptions();
            retry_policy_options.TransientFaultFilters.Add(new SqlTransientFaultFilter());

            DefaultRetryPolicy = new RetryPolicy(retry_policy_options);
        }

        public SqlDataAccessService(
            string connectionString)
            : this(new ConnectionFactory<SqlConnection>(connectionString), DefaultTimeout, DefaultRetryPolicy)
        { }

        public SqlDataAccessService(
            string connectionString, 
            TimeSpan defaultCommandTimeout)
            : this(new ConnectionFactory<SqlConnection>(connectionString), defaultCommandTimeout, DefaultRetryPolicy)
        { }

        public SqlDataAccessService(
            string connectionString,
            TimeSpan defaultCommandTimeout,
            IRetryPolicy retryPolicy)
            : this(new ConnectionFactory<SqlConnection>(connectionString), defaultCommandTimeout, retryPolicy)
        { }

        public SqlDataAccessService(
            ConnectionFactory<SqlConnection> connectionFactory, 
            TimeSpan defaultCommandTimeout)
            : this(connectionFactory, defaultCommandTimeout, DefaultRetryPolicy)
        {}

        public SqlDataAccessService(
            ConnectionFactory<SqlConnection> connectionFactory,
            TimeSpan defaultCommandTimeout,
            IRetryPolicy retryPolicy)
            : base(connectionFactory, defaultCommandTimeout)
        {
            RetryPolicy = retryPolicy;
        }

        #endregion

        #region Execute Scalar Function Async // TODO: This should go to SQL Server Specific implementation

        public async Task<T> ExecuteScalarFunctionAsync<T>(string functionName)
        {
            return
                await
                ExecuteScalarFunctionAsync<T>(functionName, EmptyParameters, DefaultExecuteOptions);
        }

        public async Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IExecuteOptions options)
        {
            return
                await
                ExecuteScalarFunctionAsync<T>(functionName, EmptyParameters, options);
        }

        public async Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters)
        {
            return
                await
                ExecuteScalarFunctionAsync<T>(functionName, parameters, DefaultExecuteOptions);
        }

        public async Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options)
        {
            var resultParameter = CreateParameter("", null);
            resultParameter.Direction = ParameterDirection.ReturnValue;

            await ExecuteScalarInternalAsync(
                ConnectionFactory,
                options,
                CommandType.StoredProcedure,
                functionName,
                parameters.Cast<SqlParameter>().Concat(new[] { (SqlParameter) resultParameter }).ToArray());

            var result = resultParameter.Value;
            return MapToClrValue<T>(result);
        }

        #endregion

        /// <summary>
        /// Creates a SqlParameter with given name and value.
        /// clr value will automatically be converted to DB value (e.g. null to DBNull)
        /// </summary>
        /// <param name="parameterName">name of SQL Parameter</param>
        /// <param name="clrValue">value of Sql Parameter</param>
        /// <returns></returns>
        public override IDbDataParameter CreateParameter(string parameterName, object clrValue)
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

        public IDbDataParameter CreateOutParameter(string parameterName, SqlDbType  type)
        {
            var result = new SqlParameter();
            result.ParameterName = parameterName;
            result.SqlDbType = type;
            result.Direction = System.Data.ParameterDirection.Output;

            return result;
        }

        public IDbDataParameter CreateTableParameter(string parameterName, IEnumerable source, string columnName, SqlDbType columnType)
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

        public override IDbDataParameter CreateOutParameter(string parameterName, DbType type)
        {
            var result = new SqlParameter();
            result.ParameterName = parameterName;
            result.DbType = type;
            result.Direction = System.Data.ParameterDirection.Output;

            return result;
        }

        #region Check XXX Exists

        public async Task<bool> CheckStoredProcedureExists(string storedProcedureName)
        {
            // "P" means Stored Procedure, see http://msdn.microsoft.com/en-us/library/ms190324.aspx
            return await CheckDatabaseObjectExistsAsync(storedProcedureName, "P");
        }

        public async Task<bool> CheckScalarFunctionExists(string functionName)
        {
            // "FN" means Scalar Function, see http://msdn.microsoft.com/en-us/library/ms190324.aspx
            return await CheckDatabaseObjectExistsAsync(functionName, "FN");
        }

        public Task<bool> CheckViewExistsAsync(string viewName)
        {
            // "V" means View, see http://msdn.microsoft.com/en-us/library/ms190324.aspx
            return CheckDatabaseObjectExistsAsync(viewName, "V");
        }

        /// <summary>
        /// see http://msdn.microsoft.com/en-us/library/ms190324.aspx
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public async Task<bool> CheckDatabaseObjectExistsAsync(string objectName, string objectType = null)
        {
            if (objectType == null)
            {
                return await ExecuteScalarTextAsync<bool>($"select case when OBJECT_ID('{objectName}') IS NULL then 0 else 1 end");
            }
            else
            {
                return await ExecuteScalarTextAsync<bool>($"select case when OBJECT_ID('{objectName}', '{objectType}') IS NULL then 0 else 1 end");
            }
        }

        #endregion
    }
}
