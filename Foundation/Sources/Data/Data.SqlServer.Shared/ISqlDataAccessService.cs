using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Text;

namespace SquaredInfinity.Data.SqlServer
{
    public interface ISqlDataAccessService : IDataAccessService
    {
        #region Execute Scalar Function

        Task<T> ExecuteScalarFunctionAsync<T>(string functionName);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IExecuteOptions options);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options);

        #endregion

        #region Check XXX Exists

        Task<bool> CheckStoredProcedureExists(string storedProcedureName);
        Task<bool> CheckScalarFunctionExists(string functionName);
        Task<bool> CheckViewExistsAsync(string viewName);

        #endregion
    }
}
