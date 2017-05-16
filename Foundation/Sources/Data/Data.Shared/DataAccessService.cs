﻿using System;
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

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(
            string procName)
        {
            return
                await
                ExecuteAllReaderAsync(
                    procName, 
                    EmptyParameters, 
                    DefaultExecuteReaderOptions)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(
            string procName,
            IExecuteReaderOptions options)
        {
            return
                await
                ExecuteAllReaderAsync(
                    procName, 
                    EmptyParameters, 
                    options)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(
            string procName,
            IReadOnlyList<IDbDataParameter> parameters)
        {
            return
                await
                ExecuteAllReaderAsync(
                    procName, 
                    parameters, 
                    DefaultExecuteReaderOptions)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(
            string procName,
            IReadOnlyList<IDbDataParameter> parameters,
            IExecuteReaderOptions options)
        {
            return
                await ExecuteAllReaderWithRetryInternalAsync(
                    ConnectionFactory,
                    options,
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
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        #endregion

        #region Execute All Reader Async (To Entity)

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(
            string procName,
            Func<IDataReader, TEntity> createEntity)
        {
            return
                await
                ExecuteAllReaderAsync(
                    procName, 
                    EmptyParameters,
                    DefaultExecuteReaderOptions, 
                    createEntity)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(
            string procName,
            IExecuteReaderOptions options,
            Func<IDataReader, TEntity> createEntity)
        {
            return
                await
                ExecuteAllReaderAsync(
                    procName, 
                    EmptyParameters,
                    DefaultExecuteReaderOptions, 
                    createEntity)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(
            string procName,
            IReadOnlyList<IDbDataParameter> parameters,
            Func<IDataReader, TEntity> createEntity)
        {
            return
               await
               ExecuteAllReaderAsync(
                   procName,
                   parameters, 
                   DefaultExecuteReaderOptions, 
                   createEntity)
               .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(
            string procName, 
            IReadOnlyList<IDbDataParameter> parameters, 
            IExecuteReaderOptions options, 
            Func<IDataReader, TEntity> createEntity)
        {
            return
               await
               ExecuteAllReaderWithRetryInternalAsync(
                   ConnectionFactory,
                   options,
                   procName,
                   parameters.Cast<TParameter>().ToArray(),
                   createEntity)
               .ConfigureAwait(continueOnCapturedContext: false);
        }

        #endregion

        #region Execute All Reader Async (Internal)

        protected async Task<IReadOnlyList<TEntity>> ExecuteAllReaderWithRetryInternalAsync<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            var retryOptions = new RetryPolicyOptions
            {
                CancellationToken = options.CancellationToken
            };

            return
                await
                ExecuteAllReaderWithRetryInternalAsync(connectionFactory, options, retryOptions, procName, parameters, createEntity)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        protected async Task<IReadOnlyList<TEntity>> ExecuteAllReaderWithRetryInternalAsync<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            IRetryPolicyOptions retryOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return
                await
                RetryPolicy
                .ExecuteAsync(retryOptions, async () =>
                {
                    return
                    await
                    ExecuteAllReaderInternalAsync(
                        ConnectionFactory,
                        options,
                        procName,
                        parameters,
                        createEntity)
                        .ConfigureAwait(continueOnCapturedContext: false);

                }).ConfigureAwait(continueOnCapturedContext: false);
        }

        protected virtual async Task<IReadOnlyList<TEntity>> ExecuteAllReaderInternalAsync<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            options.CancellationToken.ThrowIfCancellationRequested();

            var all_results = new List<TEntity>(capacity: options.ExpectedResultCount);

            using (var connection = connectionFactory.GetNewConnection())
            {
                if (options.ShouldAsyncOpenConnection)
                {
                    await
                        connection
                        .OpenAsync(options.CancellationToken)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }
                else
                {
                    connection.Open();
                }

                options.CancellationToken.ThrowIfCancellationRequested();

                using (var cmd = PrepareCommand(connection, CommandType.StoredProcedure, procName, parameters))
                {
                    if (options.ShouldAsyncExecuteCommand)
                    {
                        var reader =
                            (TDataReader)
                            await
                            cmd.ExecuteReaderAsync(options.CancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);

                        while (await reader.ReadAsync(options.CancellationToken).ConfigureAwait(continueOnCapturedContext: false))
                        {
                            options.CancellationToken.ThrowIfCancellationRequested();

                            var result = createEntity(reader);

                            all_results.Add(result);
                        }
                    }
                    else
                    {
                        var reader = (TDataReader)cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            options.CancellationToken.ThrowIfCancellationRequested();

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

        public async Task ExecuteReaderAsync(
            string procName,
            Action<IDataReader, CancellationToken> processRow)
        {
            await
                ExecuteReaderAsync(procName, EmptyParameters, DefaultExecuteReaderOptions, processRow)
                .ConfigureAwait(continueOnCapturedContext: false);
        }
        public async Task ExecuteReaderAsync(
            string procName,
            IExecuteReaderOptions options,
            Action<IDataReader, CancellationToken> processRow)
        {
            await
                ExecuteReaderAsync(procName, EmptyParameters, options, processRow)
                .ConfigureAwait(continueOnCapturedContext: false);
        }
        public async Task ExecuteReaderAsync(
            string procName,
            IReadOnlyList<IDbDataParameter> parameters,
            Action<IDataReader, CancellationToken> processRow)
        {
            await
                ExecuteReaderAsync(procName, parameters, DefaultExecuteReaderOptions, processRow)
                .ConfigureAwait(continueOnCapturedContext: false);
        }
        public async Task ExecuteReaderAsync(
            string procName,
            IReadOnlyList<IDbDataParameter> parameters,
            IExecuteReaderOptions options,
            Action<IDataReader, CancellationToken> processRow)
        {
            await
            ExecuteReaderWithRetryInternalAsync(
                ConnectionFactory,
                options,
                procName,
                parameters.Cast<TParameter>().ToArray(),
                processRow)
            .ConfigureAwait(continueOnCapturedContext: false);
        }

        #endregion

        #region Execute Reader Async (Internal)

        protected async Task ExecuteReaderWithRetryInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Action<IDataReader, CancellationToken> processRow)
        {
            var retryOptions = new RetryPolicyOptions
            {
                CancellationToken = options.CancellationToken
            };

            await
                ExecuteReaderWithRetryInternalAsync(connectionFactory, options, retryOptions, procName, parameters, processRow)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        protected async Task ExecuteReaderWithRetryInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            IRetryPolicyOptions retryOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Action<IDataReader, CancellationToken> processRow)
        {
            await
            RetryPolicy
            .ExecuteAsync(retryOptions, async () =>
            {
                await
                ExecuteReaderInternalAsync(
                    ConnectionFactory,
                    options,
                    procName,
                    parameters,
                    processRow)
                    .ConfigureAwait(continueOnCapturedContext: false);

            }).ConfigureAwait(continueOnCapturedContext: false);
        }

        protected virtual async Task ExecuteReaderInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Action<IDataReader, CancellationToken> processRow)
        {
            options.CancellationToken.ThrowIfCancellationRequested();

            using (var connection = connectionFactory.GetNewConnection())
            {
                if (options.ShouldAsyncOpenConnection)
                {
                    await
                        connection
                        .OpenAsync(options.CancellationToken)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }
                else
                {
                    connection.Open();
                }

                options.CancellationToken.ThrowIfCancellationRequested();

                using (var cmd = PrepareCommand(connection, CommandType.StoredProcedure, procName, parameters))
                {
                    if (options.ShouldAsyncExecuteCommand)
                    {
                        var reader =
                            (TDataReader)
                            await
                            cmd.ExecuteReaderAsync(options.CancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);

                        while (await reader.ReadAsync(options.CancellationToken).ConfigureAwait(continueOnCapturedContext: false))
                        {
                            options.CancellationToken.ThrowIfCancellationRequested();

                            processRow(reader, options.CancellationToken);
                        }
                    }
                    else
                    {
                        var reader = (TDataReader)cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            options.CancellationToken.ThrowIfCancellationRequested();

                            processRow(reader, options.CancellationToken);
                        }
                    }
                }
            }

        }

        #endregion

        #region Execute Reader (To Entity)

        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, Func<IDataReader, TEntity> createEntity)
        {
            return
                ExecuteReader<TEntity>(procName, EmptyParameters, DefaultExecuteReaderOptions, createEntity);
        }

        public IEnumerable<TEntity> ExecuteReader<TEntity>(
            string procName,
            IExecuteReaderOptions options,
            Func<IDataReader, TEntity> createEntity)
        {
            return
                ExecuteReader<TEntity>(procName, EmptyParameters, options, createEntity);
        }

        public IEnumerable<TEntity> ExecuteReader<TEntity>(
            string procName,
            IReadOnlyList<IDbDataParameter> parameters,
            Func<IDataReader, TEntity> createEntity)
        {
            return
                ExecuteReader<TEntity>(procName, parameters, DefaultExecuteReaderOptions, createEntity);
        }

        public IEnumerable<TEntity> ExecuteReader<TEntity>(
            string procName,
            IReadOnlyList<IDbDataParameter> parameters,
            IExecuteReaderOptions options,
            Func<IDataReader, TEntity> createEntity)
        {
            return
                ExecuteReaderWithRetryInternal(ConnectionFactory, options, procName, parameters.Cast<TParameter>().ToArray(), createEntity);
        }

        #endregion

        #region Execute Reader (Internal)

        protected IEnumerable<TEntity> ExecuteReaderWithRetryInternal<TEntity>(
          ConnectionFactory<TConnection> connectionFactory,
          IExecuteReaderOptions options,
          string procName,
          IReadOnlyList<TParameter> parameters,
          Func<TDataReader, TEntity> createEntity)
        {
            var retryOptions = new RetryPolicyOptions
            {
                CancellationToken = options.CancellationToken
            };

            return
                ExecuteReaderWithRetryInternal(connectionFactory, options, retryOptions, procName, parameters, createEntity);
        }

        protected IEnumerable<TEntity> ExecuteReaderWithRetryInternal<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            IRetryPolicyOptions retryOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return
                RetryPolicy
                .Execute<IEnumerable<TEntity>>(retryOptions, () =>
                {
                    return
                    ExecuteReaderInternal(
                        ConnectionFactory,
                        options,
                        procName,
                        parameters,
                        createEntity);
                });
        }


        protected virtual IEnumerable<TEntity> ExecuteReaderInternal<TEntity>(
        ConnectionFactory<TConnection> connectionFactory,
        IExecuteReaderOptions options,
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
                        procName,
                        parameters,
                        createEntity));
        }

        #endregion
        


        #region Execute Async

        public async Task ExecuteAsync(string procName)
        {
            await
                ExecuteAsync(
                    procName,
                    EmptyParameters,
                    DefaultExecuteOptions)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task ExecuteAsync(string procName, IExecuteOptions options)
        {
            await
                ExecuteAsync(
                    procName,
                    EmptyParameters,
                    options)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters)
        {
            await
                ExecuteAsync(
                    procName,
                    parameters,
                    DefaultExecuteOptions)
                .ConfigureAwait(continueOnCapturedContext: false);

        }

        public async Task ExecuteAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options)
        {
            var retryOptions = new RetryPolicyOptions
            {
                CancellationToken = options.CancellationToken
            };

            await
                ExecuteInternalWithRetryAsync(
                    ConnectionFactory,
                    options,
                    retryOptions,
                    CommandType.StoredProcedure,
                    procName,
                    parameters.Cast<TParameter>().ToArray())
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        #endregion

        #region Execute Text Async

        public async Task ExecuteTextAsync(string procName)
        {
            await
                ExecuteTextAsync(
                    procName,
                    EmptyParameters,
                    DefaultExecuteOptions)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task ExecuteTextAsync(string procName, IExecuteOptions options)
        {
            await
                ExecuteTextAsync(
                    procName,
                    EmptyParameters,
                    options)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters)
        {
            await
                ExecuteTextAsync(
                    procName,
                    parameters,
                    DefaultExecuteOptions)
                .ConfigureAwait(continueOnCapturedContext: false);

        }

        public async Task ExecuteTextAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options)
        {
            var retryOptions = new RetryPolicyOptions
            {
                CancellationToken = options.CancellationToken
            };

            await
                ExecuteInternalWithRetryAsync(
                    ConnectionFactory,
                    options,
                    retryOptions,
                    CommandType.Text,
                    procName,
                    parameters.Cast<TParameter>().ToArray())
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        #endregion

        #region Execute Async (Internal)

        protected async Task ExecuteInternalWithRetryAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteOptions options,
            IRetryPolicyOptions retryOptions,
            CommandType commandType,
            string commandText,
            IReadOnlyList<TParameter> parameters)
        {
            await
            RetryPolicy
            .ExecuteAsync(retryOptions, async () =>
            {
                await
                ExecuteInternalAsync(
                    ConnectionFactory,
                    options,
                    commandType,
                    commandText,
                    parameters)
                    .ConfigureAwait(continueOnCapturedContext: false);

            }).ConfigureAwait(continueOnCapturedContext: false);
        }

        protected virtual async Task ExecuteInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteOptions options,
            CommandType commandType,
            string commandText,
            IReadOnlyList<TParameter> parameters)
        {
            options.CancellationToken.ThrowIfCancellationRequested();

            try
            {
                using (var connection = connectionFactory.GetNewConnection())
                {
                    if (options.ShouldAsyncOpenConnection)
                    {
                        await
                            connection
                            .OpenAsync(options.CancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);
                    }
                    else
                    {
                        connection.Open();
                    }

                    options.CancellationToken.ThrowIfCancellationRequested();

                    using (var cmd = PrepareCommand(connection, commandType, commandText, parameters))
                    {
                        if (options.ShouldAsyncExecuteCommand)
                        {
                            await
                                cmd.ExecuteNonQueryAsync(options.CancellationToken)
                                .ConfigureAwait(continueOnCapturedContext: false);
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
        {
            return
                await
                ExecuteScalarAsync<T>(procName, EmptyParameters, DefaultExecuteOptions);
        }

        public async Task<T> ExecuteScalarAsync<T>(string procName, IExecuteOptions options)
        {
            return
                await
                ExecuteScalarAsync<T>(procName, EmptyParameters, options);
        }

        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters)
        {
            return
                await
                ExecuteScalarAsync<T>(procName, parameters, DefaultExecuteOptions);
        }

        public async Task<T> ExecuteScalarAsync<T>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options)
        {
            var retryOptions = new RetryPolicyOptions { CancellationToken = options.CancellationToken };

            var result =
                  await ExecuteScalarWithRetryInternalAsync(
                      ConnectionFactory,
                      options,
                      retryOptions,
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
        {
            return
                await
                ExecuteScalarTextAsync<T>(sql, EmptyParameters, DefaultExecuteOptions);
        }

        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IExecuteOptions options)
        {
            return
                await
                ExecuteScalarTextAsync<T>(sql, EmptyParameters, options);
        }

        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters)
        {
            return
                await
                ExecuteScalarTextAsync<T>(sql, parameters, DefaultExecuteOptions);
        }

        public async Task<T> ExecuteScalarTextAsync<T>(string sql, IReadOnlyList<IDbDataParameter> parameters, IExecuteOptions options)
        {
            var retryOptions = new RetryPolicyOptions { CancellationToken = options.CancellationToken };

            var result = 
                await
                ExecuteScalarWithRetryInternalAsync(
                    ConnectionFactory, 
                    options,
                    retryOptions,
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
            CommandType commandType,
            string commandText,
            IReadOnlyList<TParameter> parameters)
        {
            return
                await
                RetryPolicy
                .ExecuteAsync(retryOptions, async () =>
                {
                    return
                    await
                    ExecuteScalarInternalAsync(
                        ConnectionFactory,
                        options,
                        commandType,
                        commandText,
                        parameters)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }).ConfigureAwait(continueOnCapturedContext: false);
        }

        protected virtual async Task<object> ExecuteScalarInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteOptions options,
            CommandType commandType,
            string commandText,
            IReadOnlyList<TParameter> parameters)
        {
            options.CancellationToken.ThrowIfCancellationRequested();

            try
            {
                using (var connection = connectionFactory.GetNewConnection())
                {
                    if (options.ShouldAsyncOpenConnection)
                    {
                        await
                            connection
                            .OpenAsync(options.CancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);
                    }
                    else
                    {
                        connection.Open();
                    }

                    options.CancellationToken.ThrowIfCancellationRequested();

                    using (var cmd = PrepareCommand(connection, commandType, commandText, parameters))
                    {
                        var result = (object)null;

                        if (options.ShouldAsyncExecuteCommand)
                        {
                            result =
                                await
                                cmd.ExecuteScalarAsync(options.CancellationToken)
                                .ConfigureAwait(continueOnCapturedContext: false);
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
        {
            await
                ExecuteAsync(doExecute, DefaultExecuteOptions);
        }

        public async Task ExecuteAsync(Func<IDbConnection, CancellationToken, Task> doExecute, IExecuteOptions options)
        {
            var retryOptions = new RetryPolicyOptions { CancellationToken = options.CancellationToken };

            await
                ExecuteWithRetryInternalAsync(
                    ConnectionFactory,
                    options,
                    retryOptions,
                    doExecute)
                    .ConfigureAwait(continueOnCapturedContext: false);
        }

        #endregion

        #region Execute Async (Internal)

        protected async Task ExecuteWithRetryInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteOptions options,
            IRetryPolicyOptions retryOptions,
            Func<IDbConnection, CancellationToken, Task> doExecute)
        {
            await
                RetryPolicy
                .ExecuteAsync(retryOptions, async () =>
                {
                    await
                    ExecuteInternalAsync(
                        ConnectionFactory,
                        options,
                        doExecute)
                        .ConfigureAwait(continueOnCapturedContext: false);
                }).ConfigureAwait(continueOnCapturedContext: false);
        }

        protected virtual async Task ExecuteInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteOptions options,
            Func<IDbConnection, CancellationToken, Task> doExecute)
        {
            options.CancellationToken.ThrowIfCancellationRequested();

            try
            {
                using (var connection = connectionFactory.GetNewConnection())
                {
                    if (options.ShouldAsyncOpenConnection)
                    {
                        await
                            connection
                            .OpenAsync(options.CancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);
                    }
                    else
                    {
                        connection.Open();
                    }

                    options.CancellationToken.ThrowIfCancellationRequested();

                    if (options.ShouldAsyncExecuteCommand)
                    {
                        await doExecute(connection, options.CancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);
                    }
                    else
                    {
                        doExecute(connection, options.CancellationToken)
                            .Wait(options.CancellationToken);
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


        protected internal virtual TCommand PrepareCommand(TConnection connection, CommandType commandType, string commandText, IEnumerable<TParameter> parameters)
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
            command.CommandTimeout = (int)DefaultCommandTimeout.TotalMilliseconds;

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