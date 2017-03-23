using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Data
{
    public interface IDataAccessService
    {
        #region Execute All Reader Async (To Dictionary)

        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options);

        #endregion

        #region Execute All Reader Async (To Entity)

        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);

        #endregion

        #region Execute Reader Async (Process Each Row)

        Task ExecuteReaderAsync(string procName, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Action<IDataReader, CancellationToken> processRow);

        #endregion

        #region Execute Reader (To Entity)

        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);

        #endregion


        #region Execute (Proc) Async

        Task ExecuteAsync(string procName);
        Task ExecuteAsync(string procName, IExecuteOptions options);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options);

        #endregion

        #region Execute Text Async

        Task ExecuteTextAsync(string sql);
        Task ExecuteTextAsync(string sql, IExecuteOptions options);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options);

        #endregion

        #region Execute Async

        Task ExecuteAsync(Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(Func<IDbConnection, CancellationToken, Task> doExecute, IExecuteOptions options);

        #endregion


        #region Execute Scalar Async

        Task<T> ExecuteScalarAsync<T>(string procName);
        Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options);

        #endregion
        
        #region Execute Scalar Text Async

        Task<T> ExecuteScalarTextAsync<T>(string sql);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options);

        #endregion
    }
}
