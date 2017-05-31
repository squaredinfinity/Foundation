using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Text;
using System.Collections;
using System.Threading;
using SquaredInfinity.Threading;

namespace SquaredInfinity.Data.SqlServer
{
    public interface ISqlDataAccessService : IDataAccessService
    {
        #region Execute Scalar Function

        Task<T> ExecuteScalarFunctionAsync<T>(string functionName);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, TimeSpan timeout);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, TimeSpan timeout, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, int millisecondsTimeout);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, int millisecondsTimeout, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, AsyncOptions asyncOptions);

        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IExecuteOptions options);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IExecuteOptions options, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IExecuteOptions options, TimeSpan timeout);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IExecuteOptions options, TimeSpan timeout, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IExecuteOptions options, int millisecondsTimeout);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IExecuteOptions options, AsyncOptions asyncOptions);

        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions);

        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, AsyncOptions asyncOptions);

        #endregion

        #region Check XXX Exists

        Task<bool> CheckStoredProcedureExists(string storedProcedureName);
        Task<bool> CheckScalarFunctionExists(string functionName);
        Task<bool> CheckViewExistsAsync(string viewName);

        #endregion

        IDbDataParameter CreateOutParameter(string parameterName, SqlDbType type);

        IDbDataParameter CreateTableParameter(string parameterName, IEnumerable source, string columnName, SqlDbType columnType);
    }
}
