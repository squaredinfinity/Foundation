using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Text;

namespace SquaredInfinity.Data.SqlServer
{
    class ISqlDataAccessService
    {
        #region Execute Scalar Function

        Task<T> ExecuteScalarFunctionAsync<T>(string functionName);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IExecuteOptions options);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters);
        Task<T> ExecuteScalarFunctionAsync<T>(string functionName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options);

        #endregion
    }
}
