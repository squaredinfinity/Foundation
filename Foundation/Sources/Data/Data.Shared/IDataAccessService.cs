using SquaredInfinity.Threading;
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
        TimeSpan DefaultCommandTimeout { get; set; }

        #region Execute All Reader Async (To Dictionary)

        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, TimeSpan timeout);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, TimeSpan timeout, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, int millisecondsTimeout);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, int millisecondsTimeout, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, AsyncOptions asyncOptions);

        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, TimeSpan timeout);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, int millisecondsTimeout);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, AsyncOptions asyncOptions);

        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions);

        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, AsyncOptions asyncOptions);

        #endregion

        #region Execute All Reader Async (To Entity)

        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, TimeSpan timeout, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, AsyncOptions asyncOptions, Func<IDataReader, TEntity> createEntity);

        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, TimeSpan timeout, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, AsyncOptions asyncOptions, Func<IDataReader, TEntity> createEntity);


        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions, Func<IDataReader, TEntity> createEntity);


        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, AsyncOptions asyncOptions, Func<IDataReader, TEntity> createEntity);

        #endregion

        #region Execute Reader Async (Process Each Row)

        Task ExecuteReaderAsync(string procName, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, TimeSpan timeout, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, TimeSpan timeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, int millisecondsTimeout, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, int millisecondsTimeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, AsyncOptions asyncOptions, Action<IDataReader, CancellationToken> processRow);


        Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, TimeSpan timeout, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, int millisecondsTimeout, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, AsyncOptions asyncOptions, Action<IDataReader, CancellationToken> processRow);

        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions, Action<IDataReader, CancellationToken> processRow);

        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, AsyncOptions asyncOptions, Action<IDataReader, CancellationToken> processRow);

        #endregion

        #region Execute Reader (To Entity)

        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, TimeSpan timeout, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, SyncOptions syncOptions, Func<IDataReader, TEntity> createEntity);

        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, TimeSpan timeout, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, SyncOptions syncOptions, Func<IDataReader, TEntity> createEntity);

        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, SyncOptions syncOptions, Func<IDataReader, TEntity> createEntity);

        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, SyncOptions syncOptions, Func<IDataReader, TEntity> createEntity);

        #endregion


        #region Execute (Proc) Async

        Task ExecuteAsync(string procName);
        Task ExecuteAsync(string procName, CancellationToken ct);
        Task ExecuteAsync(string procName, TimeSpan timeout);
        Task ExecuteAsync(string procName, TimeSpan timeout, CancellationToken ct);
        Task ExecuteAsync(string procName, int millisecondsTimeout);
        Task ExecuteAsync(string procName, int millisecondsTimeout, CancellationToken ct);
        Task ExecuteAsync(string procName, AsyncOptions asyncOptions);

        Task ExecuteAsync(string procName, IExecuteOptions options);
        Task ExecuteAsync(string procName, IExecuteOptions options, CancellationToken ct);
        Task ExecuteAsync(string procName, IExecuteOptions options, TimeSpan timeout);
        Task ExecuteAsync(string procName, IExecuteOptions options, TimeSpan timeout, CancellationToken ct);
        Task ExecuteAsync(string procName, IExecuteOptions options, int millisecondsTimeout);
        Task ExecuteAsync(string procName, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct);
        Task ExecuteAsync(string procName, IExecuteOptions options, AsyncOptions asyncOptions);

        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions);

        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, CancellationToken ct);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout, CancellationToken ct);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct);
        Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, AsyncOptions asyncOptions);

        #endregion

        #region Execute Text Async

        Task ExecuteTextAsync(string sql);
        Task ExecuteTextAsync(string sql, CancellationToken ct);
        Task ExecuteTextAsync(string sql, TimeSpan timeout);
        Task ExecuteTextAsync(string sql, TimeSpan timeout, CancellationToken ct);
        Task ExecuteTextAsync(string sql, int millisecondsTimeout);
        Task ExecuteTextAsync(string sql, int millisecondsTimeout, CancellationToken ct);
        Task ExecuteTextAsync(string sql, AsyncOptions asyncOptions);

        Task ExecuteTextAsync(string sql, IExecuteOptions options);
        Task ExecuteTextAsync(string sql, IExecuteOptions options, CancellationToken ct);
        Task ExecuteTextAsync(string sql, IExecuteOptions options, TimeSpan timeout);
        Task ExecuteTextAsync(string sql, IExecuteOptions options, TimeSpan timeout, CancellationToken ct);
        Task ExecuteTextAsync(string sql, IExecuteOptions options, int millisecondsTimeout);
        Task ExecuteTextAsync(string sql, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct);
        Task ExecuteTextAsync(string sql, IExecuteOptions options, AsyncOptions asyncOptions);

        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions);

        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, CancellationToken ct);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout, CancellationToken ct);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct);
        Task ExecuteTextAsync(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, AsyncOptions asyncOptions);

        #endregion

        #region Execute Async

        Task ExecuteAsync(Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(TimeSpan timeout, Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(TimeSpan timeout, CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(int millisecondsTimeout, Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(int millisecondsTimeout, CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(AsyncOptions asyncOptions, Func<IDbConnection, CancellationToken, Task> doExecute);

        Task ExecuteAsync(IExecuteOptions options, Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(IExecuteOptions options, CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(IExecuteOptions options, TimeSpan timeout, Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(IExecuteOptions options, TimeSpan timeout, CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(IExecuteOptions options, int millisecondsTimeout, Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(IExecuteOptions options, int millisecondsTimeout, CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute);
        Task ExecuteAsync(IExecuteOptions options, AsyncOptions asyncOptions, Func<IDbConnection, CancellationToken, Task> doExecute);

        #endregion

        #region Execute Scalar Async

        Task<T> ExecuteScalarAsync<T>(string procName);
        Task<T> ExecuteScalarAsync<T>(string procName, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, TimeSpan timeout);
        Task<T> ExecuteScalarAsync<T>(string procName, TimeSpan timeout, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, int millisecondsTimeout);
        Task<T> ExecuteScalarAsync<T>(string procName, int millisecondsTimeout, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, AsyncOptions asyncOptions);

        Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options);
        Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, TimeSpan timeout);
        Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, TimeSpan timeout, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, int millisecondsTimeout);
        Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, AsyncOptions asyncOptions);

        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions);

        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct);
        Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, AsyncOptions asyncOptions);

        #endregion

        #region Execute Scalar Text Async

        Task<T> ExecuteScalarTextAsync<T>(string sql);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, CancellationToken ct);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, TimeSpan timeout);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, TimeSpan timeout, CancellationToken ct);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, int millisecondsTimeout);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, AsyncOptions asyncOptions);

        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions);

        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, CancellationToken ct);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout, CancellationToken ct);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct);
        Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, AsyncOptions asyncOptions);

        #endregion

        IDbDataParameter CreateParameter(string parameterName, object clrValue);

        IDbDataParameter CreateOutParameter(string parameterName, DbType type);
    }
}
