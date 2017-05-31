using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SquaredInfinity.Extensions;
using System.Collections;
using System.Threading;
using System.Dynamic;
using System.Collections.ObjectModel;
using SquaredInfinity.Collections;
using SquaredInfinity.Threading;

namespace SquaredInfinity.Data
{
    public abstract partial class DataAccessService<TConnection, TCommand, TParameter, TDataReader> : IDataAccessService
        where TConnection : DbConnection, new()
        where TCommand : DbCommand
        where TParameter : DbParameter
        where TDataReader : DbDataReader
    {

        protected readonly IReadOnlyList<IDbDataParameter> EmptyParameters = new IDbDataParameter[0];
        protected readonly IReadOnlyList<TParameter> EmptyTParameters = new TParameter[0];

        protected ConnectionFactory<TConnection> ConnectionFactory { get; set; }

        IDatabaseObjectNameResolver _databaseObjectNameResolver;
        public IDatabaseObjectNameResolver DatabaseObjectNameResolver
        {
            get { return _databaseObjectNameResolver; }
            set { _databaseObjectNameResolver = value; }
        }

        public TimeSpan DefaultCommandTimeout { get; set; }

        public IRetryPolicy RetryPolicy { get; set; } = new RetryPolicy();
        public IExecuteOptions DefaultExecuteOptions { get; set; } = new ExecuteOptions();
        public IExecuteReaderOptions DefaultExecuteReaderOptions { get; set; } = new ExecuteReaderOptions();

        #region Constructors

        public DataAccessService(
            ConnectionFactory<TConnection> connectionFactory,
            TimeSpan defaultCommandTimeout)
            : this(connectionFactory, defaultCommandTimeout, new RetryPolicy())
        { }

        public DataAccessService(
            ConnectionFactory<TConnection> connectionFactory,
            TimeSpan defaultCommandTimeout,
            IRetryPolicy retryPolicy)
        {
            ConnectionFactory = connectionFactory;
            DefaultCommandTimeout = defaultCommandTimeout;
            RetryPolicy = retryPolicy;
        }

        #endregion

        #region Execute All Reader Async (To Dictionary)

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, TimeSpan timeout)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, TimeSpan timeout, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout, ct));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, int millisecondsTimeout)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout, ct));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, AsyncOptions asyncOptions)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, asyncOptions);

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, TimeSpan timeout)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, options, new AsyncOptions(timeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, options, new AsyncOptions(timeout, ct));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, int millisecondsTimeout)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, options, new AsyncOptions(millisecondsTimeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, options, new AsyncOptions(millisecondsTimeout, ct));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options, AsyncOptions asyncOptions)
            => await ExecuteAllReaderAsync(procName, EmptyParameters, options, asyncOptions);


        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters)
            => await ExecuteAllReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout)
            => await ExecuteAllReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout, ct));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout)
            => await ExecuteAllReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout, ct));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions)
            => await ExecuteAllReaderAsync(procName, parameters, DefaultExecuteReaderOptions, asyncOptions);

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options)
            => await ExecuteAllReaderAsync(procName, parameters, options, new AsyncOptions(DefaultCommandTimeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, parameters, options, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout)
            => await ExecuteAllReaderAsync(procName, parameters, options, new AsyncOptions(timeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, parameters, options, new AsyncOptions(timeout, ct));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout)
            => await ExecuteAllReaderAsync(procName, parameters, options, new AsyncOptions(millisecondsTimeout));
        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteAllReaderAsync(procName, parameters, options, new AsyncOptions(millisecondsTimeout, ct));

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, AsyncOptions asyncOptions)
        {
            return
                await ExecuteAllReaderWithRetryInternalAsync(
                    ConnectionFactory,
                    options,
                    asyncOptions,
                    procName,
                    parameters.Cast<TParameter>().ToArray(),
                    (r) =>
                    {
                        var result = new Dictionary<string, object>(capacity: r.FieldCount);

                        for (int i = 0; i < r.FieldCount; i++)
                        {
                            result.Add(r.GetName(i), r[i]);
                        }

                        return new ReadOnlyDictionary<string, object>(result);
                    })
                .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
        }

        #endregion

        #region Execute All Reader Async (To Entity)

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout, ct), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, TimeSpan timeout, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout, ct), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout, ct), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, AsyncOptions asyncOptions, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, asyncOptions, createEntity);


        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity) 
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout, ct), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, TimeSpan timeout, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout, ct), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout, ct), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, AsyncOptions asyncOptions, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, EmptyTParameters, DefaultExecuteReaderOptions, asyncOptions, createEntity);

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, Func<IDataReader, TEntity> createEntity) 
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout, ct), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), DefaultExecuteReaderOptions, new AsyncOptions(timeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), DefaultExecuteReaderOptions, new AsyncOptions(timeout, ct), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout, ct), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), DefaultExecuteReaderOptions, asyncOptions, createEntity);

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), options, new AsyncOptions(DefaultCommandTimeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), options, new AsyncOptions(DefaultCommandTimeout, ct), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), options, new AsyncOptions(timeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), options, new AsyncOptions(timeout, ct), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), options, new AsyncOptions(millisecondsTimeout), createEntity);
        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderAsync(procName, parameters.Cast<TParameter>().ToArray(), options, new AsyncOptions(millisecondsTimeout, ct), createEntity);

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, AsyncOptions asyncOptions, Func<IDataReader, TEntity> createEntity)
            => await ExecuteAllReaderWithRetryInternalAsync(ConnectionFactory, options, asyncOptions, procName, parameters.Cast<TParameter>().ToArray(), createEntity);


        #endregion

        #region Execute All Reader Async (Internal)

        protected async Task<IReadOnlyList<TEntity>> ExecuteAllReaderWithRetryInternalAsync<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            AsyncOptions asyncOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            var retryOptions = RetryPolicyOptions.Default;

            return
                await
                ExecuteAllReaderWithRetryInternalAsync(connectionFactory, options, retryOptions, asyncOptions, procName, parameters, createEntity)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        protected async Task<IReadOnlyList<TEntity>> ExecuteAllReaderWithRetryInternalAsync<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            IRetryPolicyOptions retryOptions,
            AsyncOptions asyncOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return
                await
                RetryPolicy
                .ExecuteAsync(retryOptions, asyncOptions.CancellationToken, async () =>
                {
                    return
                    await
                    ExecuteAllReaderInternalAsync(
                        ConnectionFactory,
                        options,
                        asyncOptions,
                        procName,
                        parameters,
                        createEntity)
                        .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);

                }).ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
        }

        protected virtual async Task<IReadOnlyList<TEntity>> ExecuteAllReaderInternalAsync<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            AsyncOptions asyncOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            asyncOptions.CancellationToken.ThrowIfCancellationRequested();

            var all_results = new List<TEntity>(capacity: options.ExpectedResultCount);

            using (var connection = connectionFactory.GetNewConnection())
            {
                if (options.ShouldAsyncOpenConnection)
                {
                    await
                        connection
                        .OpenAsync(asyncOptions.CancellationToken)
                        .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
                }
                else
                {
                    connection.Open();
                }

                asyncOptions.CancellationToken.ThrowIfCancellationRequested();

                using (var cmd = PrepareCommand(connection, asyncOptions.MillisecondsTimeout, CommandType.StoredProcedure, procName, parameters))
                {
                    if (options.ShouldAsyncExecuteCommand)
                    {
                        var reader =
                            (TDataReader)
                            await
                            cmd.ExecuteReaderAsync(asyncOptions.CancellationToken)
                            .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);

                        while (await reader.ReadAsync(asyncOptions.CancellationToken).ConfigureAwait(asyncOptions.ContinueOnCapturedContext))
                        {
                            asyncOptions.CancellationToken.ThrowIfCancellationRequested();

                            var result = createEntity(reader);

                            all_results.Add(result);
                        }
                    }
                    else
                    {
                        var reader = (TDataReader)cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            asyncOptions.CancellationToken.ThrowIfCancellationRequested();

                            var result = createEntity(reader);

                            all_results.Add(result);
                        }
                    }
                }
            }

            all_results.TrimExcess();

            return all_results;
        }

        #endregion

        #region Execute Reader Async (Process Each Row)

        public async Task ExecuteReaderAsync(string procName, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout), processRow);
        public async Task ExecuteReaderAsync(string procName, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout, ct), processRow);
        public async Task ExecuteReaderAsync(string procName, TimeSpan timeout, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout), processRow);
        public async Task ExecuteReaderAsync(string procName, TimeSpan timeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout, ct), processRow);
        public async Task ExecuteReaderAsync(string procName, int millisecondsTimeout, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout), processRow);
        public async Task ExecuteReaderAsync(string procName, int millisecondsTimeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout, ct), processRow);
        public async Task ExecuteReaderAsync(string procName, AsyncOptions asyncOptions, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, asyncOptions, processRow);


        public async Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout), processRow);
        public async Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout, ct), processRow);
        public async Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, TimeSpan timeout, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, options, new AsyncOptions(timeout), processRow);
        public async Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, options, new AsyncOptions(timeout, ct), processRow);
        public async Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, int millisecondsTimeout, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, options, new AsyncOptions(millisecondsTimeout), processRow);
        public async Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, options, new AsyncOptions(millisecondsTimeout, ct), processRow);
        public async Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, AsyncOptions asyncOptions, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, EmptyParameters, options, asyncOptions, processRow);

        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout), processRow);
        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(DefaultCommandTimeout, ct), processRow);
        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout), processRow);
        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(timeout, ct), processRow);
        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout), processRow);
        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, DefaultExecuteReaderOptions, new AsyncOptions(millisecondsTimeout, ct), processRow);
        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, DefaultExecuteReaderOptions, asyncOptions, processRow);


        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, options, new AsyncOptions(DefaultCommandTimeout), processRow);
        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
                    => await ExecuteReaderAsync(procName, parameters, options, new AsyncOptions(DefaultCommandTimeout, ct), processRow);
        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, options, new AsyncOptions(timeout), processRow);
        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, options, new AsyncOptions(timeout, ct), processRow);
        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, options, new AsyncOptions(millisecondsTimeout), processRow);
        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderAsync(procName, parameters, options, new AsyncOptions(millisecondsTimeout, ct), processRow);

        public async Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, AsyncOptions asyncOptions, Action<IDataReader, CancellationToken> processRow)
            => await ExecuteReaderWithRetryInternalAsync(ConnectionFactory, options, asyncOptions, procName, parameters.Cast<TParameter>().ToArray(), processRow);

        #endregion

        #region Execute Reader Async (Internal)

        protected async Task ExecuteReaderWithRetryInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            AsyncOptions asyncOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Action<IDataReader, CancellationToken> processRow)
        {
            var retryOptions = RetryPolicyOptions.Default;

            await
                ExecuteReaderWithRetryInternalAsync(connectionFactory, options, retryOptions, asyncOptions, procName, parameters, processRow)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        protected async Task ExecuteReaderWithRetryInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            IRetryPolicyOptions retryOptions,
            AsyncOptions asyncOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Action<IDataReader, CancellationToken> processRow)
        {
            await
            RetryPolicy
            .ExecuteAsync(retryOptions, asyncOptions.CancellationToken, async () =>
            {
                await
                ExecuteReaderInternalAsync(
                    ConnectionFactory,
                    options,
                    asyncOptions,
                    procName,
                    parameters,
                    processRow)
                    .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);

            }).ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
        }

        protected virtual async Task ExecuteReaderInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            AsyncOptions asyncOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Action<IDataReader, CancellationToken> processRow)
        {
            asyncOptions.CancellationToken.ThrowIfCancellationRequested();

            using (var connection = connectionFactory.GetNewConnection())
            {
                if (options.ShouldAsyncOpenConnection)
                {
                    await
                        connection
                        .OpenAsync(asyncOptions.CancellationToken)
                        .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
                }
                else
                {
                    connection.Open();
                }

                asyncOptions.CancellationToken.ThrowIfCancellationRequested();

                using (var cmd = PrepareCommand(connection, asyncOptions.MillisecondsTimeout, CommandType.StoredProcedure, procName, parameters))
                {
                    if (options.ShouldAsyncExecuteCommand)
                    {
                        var reader =
                            (TDataReader)
                            await
                            cmd.ExecuteReaderAsync(asyncOptions.CancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);

                        while (await reader.ReadAsync(asyncOptions.CancellationToken).ConfigureAwait(asyncOptions.ContinueOnCapturedContext))
                        {
                            asyncOptions.CancellationToken.ThrowIfCancellationRequested();

                            processRow(reader, asyncOptions.CancellationToken);
                        }
                    }
                    else
                    {
                        var reader = (TDataReader)cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            asyncOptions.CancellationToken.ThrowIfCancellationRequested();

                            processRow(reader, asyncOptions.CancellationToken);
                        }
                    }
                }
            }

        }

        #endregion

        #region Execute Reader (To Entity)

        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, DefaultExecuteReaderOptions, new SyncOptions(DefaultCommandTimeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, DefaultExecuteReaderOptions, new SyncOptions(DefaultCommandTimeout, ct), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, TimeSpan timeout, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, DefaultExecuteReaderOptions, new SyncOptions(timeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, DefaultExecuteReaderOptions, new SyncOptions(timeout, ct), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, DefaultExecuteReaderOptions, new SyncOptions(millisecondsTimeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, DefaultExecuteReaderOptions, new SyncOptions(millisecondsTimeout, ct), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, SyncOptions syncOptions, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, DefaultExecuteReaderOptions, syncOptions, createEntity);

        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, options, new SyncOptions(DefaultCommandTimeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, options, new SyncOptions(DefaultCommandTimeout, ct), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, TimeSpan timeout, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, options, new SyncOptions(timeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, options, new SyncOptions(timeout, ct), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, options, new SyncOptions(millisecondsTimeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, options, new SyncOptions(millisecondsTimeout, ct), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, SyncOptions syncOptions, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, EmptyParameters, options, syncOptions, createEntity);

        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, parameters, DefaultExecuteReaderOptions, new SyncOptions(DefaultCommandTimeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, parameters, DefaultExecuteReaderOptions, new SyncOptions(DefaultCommandTimeout, ct), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, parameters, DefaultExecuteReaderOptions, new SyncOptions(timeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, parameters, DefaultExecuteReaderOptions, new SyncOptions(timeout, ct), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, parameters, DefaultExecuteReaderOptions, new SyncOptions(millisecondsTimeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, parameters, DefaultExecuteReaderOptions, new SyncOptions(millisecondsTimeout, ct), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, SyncOptions syncOptions, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader<TEntity>(procName, parameters, DefaultExecuteReaderOptions, syncOptions, createEntity);

        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader(procName, parameters, options, new SyncOptions(DefaultCommandTimeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader(procName, parameters, options, new SyncOptions(DefaultCommandTimeout, ct), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader(procName, parameters, options, new SyncOptions(timeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, TimeSpan timeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader(procName, parameters, options, new SyncOptions(timeout, ct), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader(procName, parameters, options, new SyncOptions(millisecondsTimeout), createEntity);
        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, int millisecondsTimeout, CancellationToken ct, Func<IDataReader, TEntity> createEntity)
            => ExecuteReader(procName, parameters, options, new SyncOptions(millisecondsTimeout, ct), createEntity);

        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, SyncOptions syncOptions, Func<IDataReader, TEntity> createEntity)
            => ExecuteReaderWithRetryInternal(ConnectionFactory, options, syncOptions, procName, parameters.Cast<TParameter>().ToArray(), createEntity);

        #endregion

        #region Execute Reader (Internal)

        protected IEnumerable<TEntity> ExecuteReaderWithRetryInternal<TEntity>(
          ConnectionFactory<TConnection> connectionFactory,
          IExecuteReaderOptions options,
          SyncOptions syncOptions,
          string procName,
          IReadOnlyList<TParameter> parameters,
          Func<TDataReader, TEntity> createEntity)
        {
            var retryOptions = RetryPolicyOptions.Default;

            return ExecuteReaderWithRetryInternal(connectionFactory, options, retryOptions, syncOptions, procName, parameters, createEntity);
        }

        protected IEnumerable<TEntity> ExecuteReaderWithRetryInternal<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            IRetryPolicyOptions retryOptions,
            SyncOptions syncOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return
                RetryPolicy
                .Execute<IEnumerable<TEntity>>(retryOptions, syncOptions.CancellationToken, () =>
                {
                    return
                    ExecuteReaderInternal(
                        ConnectionFactory,
                        options,
                        syncOptions,
                        procName,
                        parameters,
                        createEntity);
                });
        }


        protected virtual IEnumerable<TEntity> ExecuteReaderInternal<TEntity>(
        ConnectionFactory<TConnection> connectionFactory,
        IExecuteReaderOptions options,
        SyncOptions syncOptions,
        string procName,
        IReadOnlyList<TParameter> parameters,
        Func<TDataReader, TEntity> createEntity)
        {
            return
                new CommandResultIterator<TEntity>(
                    () =>
                    new CommandResultEnumerator<TConnection, TCommand, TDataReader, TParameter, TEntity>(
                        this,
                        connectionFactory,
                        syncOptions.MillisecondsTimeout,
                        procName,
                        parameters,
                        createEntity));
        }

        #endregion
        
        #region Execute Async

        public async Task ExecuteAsync(string procName)
            => await ExecuteAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout));
        public async Task ExecuteAsync(string procName, CancellationToken ct)
            => await ExecuteAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task ExecuteAsync(string procName, TimeSpan timeout)
            => await ExecuteAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(timeout));
        public async Task ExecuteAsync(string procName, TimeSpan timeout, CancellationToken ct)
            => await ExecuteAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(timeout, ct));
        public async Task ExecuteAsync(string procName, int millisecondsTimeout)
            => await ExecuteAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout));
        public async Task ExecuteAsync(string procName, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout, ct));
        public async Task ExecuteAsync(string procName, AsyncOptions asyncOptions)
            => await ExecuteAsync(procName, EmptyParameters, DefaultExecuteOptions, asyncOptions);

        public async Task ExecuteAsync(string procName, IExecuteOptions options)
            => await ExecuteAsync(procName, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout));
        public async Task ExecuteAsync(string procName, IExecuteOptions options, CancellationToken ct)
            => await ExecuteAsync(procName, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task ExecuteAsync(string procName, IExecuteOptions options, TimeSpan timeout)
            => await ExecuteAsync(procName, EmptyParameters, options, new AsyncOptions(timeout));
        public async Task ExecuteAsync(string procName, IExecuteOptions options, TimeSpan timeout, CancellationToken ct)
            => await ExecuteAsync(procName, EmptyParameters, options, new AsyncOptions(timeout, ct));
        public async Task ExecuteAsync(string procName, IExecuteOptions options, int millisecondsTimeout)
            => await ExecuteAsync(procName, EmptyParameters, options, new AsyncOptions(millisecondsTimeout));
        public async Task ExecuteAsync(string procName, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteAsync(procName, EmptyParameters, options, new AsyncOptions(millisecondsTimeout, ct));
        public async Task ExecuteAsync(string procName, IExecuteOptions options, AsyncOptions asyncOptions)
            => await ExecuteAsync(procName, EmptyParameters, options, asyncOptions);

        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters)
            => await ExecuteAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct)
            => await ExecuteAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout)
            => await ExecuteAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(timeout));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct)
            => await ExecuteAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(timeout, ct));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout)
            => await ExecuteAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout, ct));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions)
            => await ExecuteAsync(procName, parameters, DefaultExecuteOptions, asyncOptions);

        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options)
            => await ExecuteAsync(procName, parameters, options, new AsyncOptions(DefaultCommandTimeout));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, CancellationToken ct)
            => await ExecuteAsync(procName, parameters, options, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout)
            => await ExecuteAsync(procName, parameters, options, new AsyncOptions(timeout));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout, CancellationToken ct)
            => await ExecuteAsync(procName, parameters, options, new AsyncOptions(timeout, ct));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout)
            => await ExecuteAsync(procName, parameters, options, new AsyncOptions(millisecondsTimeout));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteAsync(procName, parameters, options, new AsyncOptions(millisecondsTimeout, ct));
        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, AsyncOptions asyncOptions)
            => await ExecuteInternalWithRetryAsync(ConnectionFactory, options, RetryPolicyOptions.Default, asyncOptions, CommandType.StoredProcedure, procName, parameters.Cast<TParameter>().ToArray());

        #endregion

        #region Execute Text Async

        public async Task ExecuteTextAsync(string procName)
            => await ExecuteTextAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout));
        public async Task ExecuteTextAsync(string procName, CancellationToken ct)
            => await ExecuteTextAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task ExecuteTextAsync(string procName, TimeSpan timeout)
            => await ExecuteTextAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(timeout));
        public async Task ExecuteTextAsync(string procName, TimeSpan timeout, CancellationToken ct)
            => await ExecuteTextAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(timeout, ct));
        public async Task ExecuteTextAsync(string procName, int millisecondsTimeout)
            => await ExecuteTextAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout));
        public async Task ExecuteTextAsync(string procName, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteTextAsync(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout, ct));
        public async Task ExecuteTextAsync(string procName, AsyncOptions asyncOptions)
            => await ExecuteTextAsync(procName, EmptyParameters, DefaultExecuteOptions, asyncOptions);

        public async Task ExecuteTextAsync(string procName, IExecuteOptions options)
            => await ExecuteTextAsync(procName, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout));
        public async Task ExecuteTextAsync(string procName, IExecuteOptions options, CancellationToken ct)
            => await ExecuteTextAsync(procName, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task ExecuteTextAsync(string procName, IExecuteOptions options, TimeSpan timeout)
            => await ExecuteTextAsync(procName, EmptyParameters, options, new AsyncOptions(timeout));
        public async Task ExecuteTextAsync(string procName, IExecuteOptions options, TimeSpan timeout, CancellationToken ct)
            => await ExecuteTextAsync(procName, EmptyParameters, options, new AsyncOptions(timeout, ct));
        public async Task ExecuteTextAsync(string procName, IExecuteOptions options, int millisecondsTimeout)
            => await ExecuteTextAsync(procName, EmptyParameters, options, new AsyncOptions(millisecondsTimeout));
        public async Task ExecuteTextAsync(string procName, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteTextAsync(procName, EmptyParameters, options, new AsyncOptions(millisecondsTimeout, ct));
        public async Task ExecuteTextAsync(string procName, IExecuteOptions options, AsyncOptions asyncOptions)
            => await ExecuteTextAsync(procName, EmptyParameters, options, asyncOptions);

        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters)
            => await ExecuteTextAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout));
        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct)
            => await ExecuteTextAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout)
            => await ExecuteTextAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(timeout));
        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct)
            => await ExecuteTextAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(timeout, ct));
        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout)
            => await ExecuteTextAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout));
        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteTextAsync(procName, parameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout, ct));
        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions)
            => await ExecuteTextAsync(procName, parameters, DefaultExecuteOptions, asyncOptions);

        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options)
            => await ExecuteTextAsync(procName, parameters, options, new AsyncOptions(DefaultCommandTimeout));
        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, CancellationToken ct)
            => await ExecuteTextAsync(procName, parameters, options, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout)
            => await ExecuteTextAsync(procName, parameters, options, new AsyncOptions(timeout));
        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout, CancellationToken ct)
            => await ExecuteTextAsync(procName, parameters, options, new AsyncOptions(timeout, ct));
        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout)
            => await ExecuteTextAsync(procName, parameters, options, new AsyncOptions(millisecondsTimeout));
        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteTextAsync(procName, parameters, options, new AsyncOptions(millisecondsTimeout, ct));


        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, AsyncOptions asyncOptions)
            => await ExecuteInternalWithRetryAsync(ConnectionFactory, options, RetryPolicyOptions.Default, asyncOptions, CommandType.Text, procName, parameters.Cast<TParameter>().ToArray());

        #endregion

        #region Execute Async (Internal)

        protected async Task ExecuteInternalWithRetryAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteOptions options,
            IRetryPolicyOptions retryOptions,
            AsyncOptions asyncOptions,
            CommandType commandType,
            string commandText,
            IReadOnlyList<TParameter> parameters)
        {
            await
            RetryPolicy
            .ExecuteAsync(retryOptions, asyncOptions.CancellationToken, async () =>
            {
                await
                ExecuteInternalAsync(
                    ConnectionFactory,
                    options,
                    asyncOptions,
                    commandType,
                    commandText,
                    parameters)
                    .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);

            }).ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
        }

        protected virtual async Task ExecuteInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteOptions options,
            AsyncOptions asyncOptions,
            CommandType commandType,
            string commandText,
            IReadOnlyList<TParameter> parameters)
        {
            asyncOptions.CancellationToken.ThrowIfCancellationRequested();

            try
            {
                using (var connection = connectionFactory.GetNewConnection())
                {
                    if (options.ShouldAsyncOpenConnection)
                    {
                        await
                            connection
                            .OpenAsync(asyncOptions.CancellationToken)
                            .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
                    }
                    else
                    {
                        connection.Open();
                    }

                    asyncOptions.CancellationToken.ThrowIfCancellationRequested();

                    using (var cmd = PrepareCommand(connection, asyncOptions.MillisecondsTimeout, commandType, commandText, parameters))
                    {
                        if (options.ShouldAsyncExecuteCommand)
                        {
                            await
                                cmd.ExecuteNonQueryAsync(asyncOptions.CancellationToken)
                                .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
                        }
                        else
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // NOTE:    reference to parameters may be held up the call stack
                        //          which will prevent command from being disposed properly and parameters from being reused.
                        //          we will now clear command's parameters to prevent this from happening
                        cmd.Parameters.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                var ex_new = new Exception("Failed during command execution.", ex);

                ex_new.TryAddContextData("connection factory", () => connectionFactory.ToString());
                ex_new.TryAddContextData("command", () => commandText);
                ex_new.TryAddContextData("parameters", () => parameters);

                throw ex_new;
            }
        }

        #endregion
        
        #region Execute Scalar Async

        public async Task<T> ExecuteScalarAsync<T>(string procName)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, TimeSpan timeout)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(timeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, TimeSpan timeout, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(timeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, int millisecondsTimeout)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, AsyncOptions asyncOptions)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, DefaultExecuteOptions, asyncOptions);

        public async Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, TimeSpan timeout)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, options, new AsyncOptions(timeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, TimeSpan timeout, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, options, new AsyncOptions(timeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, int millisecondsTimeout)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, options, new AsyncOptions(millisecondsTimeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, options, new AsyncOptions(millisecondsTimeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options, AsyncOptions asyncOptions)
            => await ExecuteScalarAsync<T>(procName, EmptyParameters, options, asyncOptions);

        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters)
            => await ExecuteScalarAsync<T>(procName, parameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, parameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout)
            => await ExecuteScalarAsync<T>(procName, parameters, DefaultExecuteOptions, new AsyncOptions(timeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, parameters, DefaultExecuteOptions, new AsyncOptions(timeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout)
            => await ExecuteScalarAsync<T>(procName, parameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, parameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions)
            => await ExecuteScalarAsync<T>(procName, parameters, DefaultExecuteOptions, asyncOptions);

        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options)
            => await ExecuteScalarAsync<T>(procName, parameters, options, new AsyncOptions(DefaultCommandTimeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, parameters, options, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout)
            => await ExecuteScalarAsync<T>(procName, parameters, options, new AsyncOptions(timeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, parameters, options, new AsyncOptions(timeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout)
            => await ExecuteScalarAsync<T>(procName, parameters, options, new AsyncOptions(millisecondsTimeout));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteScalarAsync<T>(procName, parameters, options, new AsyncOptions(millisecondsTimeout, ct));
        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, AsyncOptions asyncOptions)
        {
            var retryOptions = RetryPolicyOptions.Default;

            var result =
                  await ExecuteScalarWithRetryInternalAsync(
                      ConnectionFactory,
                      options,
                      retryOptions,
                      asyncOptions,
                      CommandType.StoredProcedure,
                      procName,
                      parameters.Cast<TParameter>().ToArray());


            if (result == null && !typeof(T).IsNullable())
                throw new InvalidCastException(
                    $"{procName} returned NULL but expected type is non-nullable {typeof(T).Name}");

            return MapToClrValue<T>(result);
        }

        #endregion

        #region Execute Scalar Text Async

        public async Task<T> ExecuteScalarTextAsync<T>(string sql)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, TimeSpan timeout)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(timeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, TimeSpan timeout, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(timeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, int millisecondsTimeout)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, AsyncOptions asyncOptions)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, DefaultExecuteOptions, asyncOptions);

        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, options, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, TimeSpan timeout)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, options, new AsyncOptions(timeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, TimeSpan timeout, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, options, new AsyncOptions(timeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, int millisecondsTimeout)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, options, new AsyncOptions(millisecondsTimeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, options, new AsyncOptions(millisecondsTimeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options, AsyncOptions asyncOptions)
            => await ExecuteScalarTextAsync<T>(sql, EmptyParameters, options, asyncOptions);

        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters)
            => await ExecuteScalarTextAsync<T>(sql, parameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, parameters, DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout)
            => await ExecuteScalarTextAsync<T>(sql, parameters, DefaultExecuteOptions, new AsyncOptions(timeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, TimeSpan timeout, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, parameters, DefaultExecuteOptions, new AsyncOptions(timeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout)
            => await ExecuteScalarTextAsync<T>(sql, parameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, parameters, DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, AsyncOptions asyncOptions)
            => await ExecuteScalarTextAsync<T>(sql, parameters, DefaultExecuteOptions, asyncOptions);

        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options)
            => await ExecuteScalarTextAsync<T>(sql, parameters, options, new AsyncOptions(DefaultCommandTimeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, parameters, options, new AsyncOptions(DefaultCommandTimeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout)
            => await ExecuteScalarTextAsync<T>(sql, parameters, options, new AsyncOptions(timeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, TimeSpan timeout, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, parameters, options, new AsyncOptions(timeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout)
            => await ExecuteScalarTextAsync<T>(sql, parameters, options, new AsyncOptions(millisecondsTimeout));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, int millisecondsTimeout, CancellationToken ct)
            => await ExecuteScalarTextAsync<T>(sql, parameters, options, new AsyncOptions(millisecondsTimeout, ct));
        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options, AsyncOptions asyncOptions)
        {
            var result = 
                await
                ExecuteScalarWithRetryInternalAsync(
                    ConnectionFactory, 
                    options,
                    RetryPolicyOptions.Default,
                    asyncOptions,
                    CommandType.Text, 
                    sql, 
                    parameters.Cast<TParameter>().ToArray());

            if (result == null && !typeof(T).IsNullable())
                throw new InvalidCastException(
                    $"{sql} returned NULL but expected type is non-nullable {typeof(T).Name}");

            return MapToClrValue<T>(result);
        }

        #endregion

        #region Execute Scalar Async (Internal)

        protected async Task<object> ExecuteScalarWithRetryInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteOptions options,
            IRetryPolicyOptions retryOptions,
            AsyncOptions asyncOptions,
            CommandType commandType,
            string commandText,
            IReadOnlyList<TParameter> parameters)
        {
            return
                await
                RetryPolicy
                .ExecuteAsync(retryOptions, asyncOptions.CancellationToken, async () =>
                {
                    return
                    await
                    ExecuteScalarInternalAsync(
                        ConnectionFactory,
                        options,
                        asyncOptions,
                        commandType,
                        commandText,
                        parameters)
                        .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
                }).ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
        }

        protected virtual async Task<object> ExecuteScalarInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteOptions options,
            AsyncOptions asyncOptions,
            CommandType commandType,
            string commandText,
            IReadOnlyList<TParameter> parameters)
        {
            asyncOptions.CancellationToken.ThrowIfCancellationRequested();

            try
            {
                using (var connection = connectionFactory.GetNewConnection())
                {
                    if (options.ShouldAsyncOpenConnection)
                    {
                        await
                            connection
                            .OpenAsync(asyncOptions.CancellationToken)
                            .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
                    }
                    else
                    {
                        connection.Open();
                    }

                    asyncOptions.CancellationToken.ThrowIfCancellationRequested();

                    using (var cmd = PrepareCommand(connection, asyncOptions.MillisecondsTimeout, commandType, commandText, parameters))
                    {
                        var result = (object)null;

                        if (options.ShouldAsyncExecuteCommand)
                        {
                            result =
                                await
                                cmd.ExecuteScalarAsync(asyncOptions.CancellationToken)
                                .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
                        }
                        else
                        {
                            result =
                                cmd.ExecuteScalar();
                        }

                        // NOTE:    reference to parameters may be held up the call stack
                        //          which will prevent command from being disposed properly and parameters from being reused.
                        //          we will now clear command's parameters to prevent this from happening
                        cmd.Parameters.Clear();

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                var ex_new = new Exception("Failed during command execution.", ex);

                ex_new.TryAddContextData("connection factory", () => connectionFactory.ToString());
                ex_new.TryAddContextData("command", () => commandText);
                ex_new.TryAddContextData("parameters", () => parameters);

                throw ex_new;
            }
        }

        #endregion

        #region Execute Async

        public async Task ExecuteAsync(Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout), doExecute);
        public async Task ExecuteAsync(CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(DefaultExecuteOptions, new AsyncOptions(DefaultCommandTimeout, ct), doExecute);
        public async Task ExecuteAsync(TimeSpan timeout, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(DefaultExecuteOptions, new AsyncOptions(timeout), doExecute);
        public async Task ExecuteAsync(TimeSpan timeout, CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(DefaultExecuteOptions, new AsyncOptions(timeout, ct), doExecute);
        public async Task ExecuteAsync(int millisecondsTimeout, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout), doExecute);
        public async Task ExecuteAsync(int millisecondsTimeout, CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(DefaultExecuteOptions, new AsyncOptions(millisecondsTimeout, ct), doExecute);
        public async Task ExecuteAsync(AsyncOptions asyncOptions, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(DefaultExecuteOptions, asyncOptions, doExecute);

        public async Task ExecuteAsync(IExecuteOptions options, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(options, new AsyncOptions(DefaultCommandTimeout), doExecute);
        public async Task ExecuteAsync(IExecuteOptions options, CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(options, new AsyncOptions(DefaultCommandTimeout, ct), doExecute);
        public async Task ExecuteAsync(IExecuteOptions options, TimeSpan timeout, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(options, new AsyncOptions(timeout), doExecute);
        public async Task ExecuteAsync(IExecuteOptions options, TimeSpan timeout, CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(options, new AsyncOptions(timeout, ct), doExecute);
        public async Task ExecuteAsync(IExecuteOptions options, int millisecondsTimeout, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(options, new AsyncOptions(millisecondsTimeout), doExecute);
        public async Task ExecuteAsync(IExecuteOptions options, int millisecondsTimeout, CancellationToken ct, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteAsync(options, new AsyncOptions(millisecondsTimeout, ct), doExecute);
        public async Task ExecuteAsync(IExecuteOptions options, AsyncOptions asyncOptions, Func<IDbConnection, CancellationToken, Task> doExecute)
            => await ExecuteWithRetryInternalAsync(ConnectionFactory, options, RetryPolicyOptions.Default, asyncOptions, doExecute);

        #endregion

        #region Execute Async (Internal)

        protected async Task ExecuteWithRetryInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteOptions options,
            IRetryPolicyOptions retryOptions,
            AsyncOptions asyncOptions,
            Func<IDbConnection, CancellationToken, Task> doExecute)
        {
            await
                RetryPolicy
                .ExecuteAsync(retryOptions, asyncOptions.CancellationToken, async () =>
                {
                    await
                    ExecuteInternalAsync(
                        ConnectionFactory,
                        options,
                        asyncOptions,
                        doExecute)
                        .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
                }).ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
        }

        protected virtual async Task ExecuteInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteOptions options,
            AsyncOptions asyncOptions,
            Func<IDbConnection, CancellationToken, Task> doExecute)
        {
            asyncOptions.CancellationToken.ThrowIfCancellationRequested();

            try
            {
                using (var connection = connectionFactory.GetNewConnection())
                {
                    if (options.ShouldAsyncOpenConnection)
                    {
                        await
                            connection
                            .OpenAsync(asyncOptions.CancellationToken)
                            .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
                    }
                    else
                    {
                        connection.Open();
                    }

                    asyncOptions.CancellationToken.ThrowIfCancellationRequested();

                    if (options.ShouldAsyncExecuteCommand)
                    {
                        await doExecute(connection, asyncOptions.CancellationToken)
                            .ConfigureAwait(asyncOptions.ContinueOnCapturedContext);
                    }
                    else
                    {
                        doExecute(connection, asyncOptions.CancellationToken)
                            .Wait(asyncOptions.MillisecondsTimeout, asyncOptions.CancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                var ex_new = new Exception("Failed during execution.", ex);

                ex_new.TryAddContextData("connection factory", () => connectionFactory.ToString());

                throw ex_new;
            }
        }

        #endregion


        protected internal virtual TCommand PrepareCommand(
            TConnection connection,
            int millisecondsCommandTimeout,
            CommandType commandType, 
            string commandText, 
            IEnumerable<TParameter> parameters)
        {
            var nameResolver = DatabaseObjectNameResolver;

            if (nameResolver != null)
            {
                if (commandType == CommandType.StoredProcedure)
                {
                    commandText = nameResolver.GetActualStoredProcedureOrFunctionName(commandText);
                }
            }

            var command = connection.CreateCommand();

            command.CommandType = commandType;
            command.CommandText = commandText;
            command.CommandTimeout = millisecondsCommandTimeout;

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return (TCommand)command;
        }


        public abstract IDbDataParameter CreateParameter(string parameterName, object clrValue);

        public abstract IDbDataParameter CreateOutParameter(string parameterName, DbType type);

        public TTarget MapToClrValue<TTarget>(object dbValue, TTarget defaultValue = default(TTarget))
        {
            TTarget clrTarget = default(TTarget);

            MapToClrValue<TTarget>(dbValue, ref clrTarget, defaultValue);

            return clrTarget;
        }

        public void MapToClrValue<TTarget>(object dbValue, TTarget target, Action<TTarget> setValue, TTarget defaultValue = default(TTarget))
        {
            TTarget clrTarget = default(TTarget);

            this.MapToClrValue<TTarget>(dbValue, ref clrTarget, defaultValue);

            setValue(clrTarget);
        }

        /// <summary>
        /// Provides a logic for mapping DB values to Clr values.
        /// For example, mapping of DBNull to null.
        /// </summary>
        /// <typeparam name="TTarget">.Net type to map to</typeparam>
        /// <param name="dbValue">DB value</param>
        /// <param name="clrTarget">.Net class or struct member where mapped value will be written to.</param>
        /// <param name="defaultValue">Value to use to substitute NULL</param>
        public virtual void MapToClrValue<TTarget>(object dbValue, ref TTarget clrTarget, TTarget defaultValue = default(TTarget))
        {
            if (dbValue == null)
            {
                clrTarget = defaultValue;
                return;
            }

            if (dbValue == DBNull.Value)
            {
                clrTarget = defaultValue;
                return;
            }

            var targetType = typeof(TTarget);
            var sourceType = dbValue.GetType();

            if (sourceType.IsTypeEquivalentTo(targetType, treatNullableAsEquivalent: true) || sourceType.ImplementsOrExtends(targetType))
            {
                clrTarget = (TTarget)dbValue;
                return;
            }

            //# Timespan
            //  Sql Server does not support TimeSpan directly
            //  Convert from number of ticks instead
            if (targetType == typeof(TimeSpan?) || targetType == typeof(TimeSpan))
            {
                clrTarget = (TTarget)(object)TimeSpan.FromTicks((long)dbValue);
                return;
            }

            //# XDocument
            //  Sql Server does not support XDocument directly
            //  Convert from string representation instead
            if (targetType == typeof(XDocument))
            {
                clrTarget = (TTarget)(object)XDocument.Parse(dbValue as string);
                return;
            }

            //# XElement
            //  Sql Server does not support XElement directly
            //  Convert from string representation instead
            if (targetType == typeof(XElement))
            {
                clrTarget = (TTarget)(object)XElement.Parse(dbValue as string);
                return;
            }

            //# IConvertible
            //  Convert DB value to .NET value if it can be converted without loss of information
            if (dbValue is IConvertible)
            {
                clrTarget = (TTarget)Convert.ChangeType(dbValue, typeof(TTarget));
            }
            else
            {
                var ex = new InvalidCastException(
                    $"Unable to Map type {sourceType.Name} to {targetType.Name}.");

                // todo: add context data AssemblyQName
            }
        }
    }
}
